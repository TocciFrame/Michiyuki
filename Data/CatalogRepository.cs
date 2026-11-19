using Michiyuki.Data.Rows;
using Michiyuki.Models;
using static Supabase.Postgrest.Constants;

namespace Michiyuki.Data;

/// <summary>
/// The only class that talks to Supabase. It turns table rows into the model classes in Models/
/// and back, so the pages never see a row. Every read filters and limits in the database
/// (Where and Range come before Get) and no query runs inside a loop.
/// </summary>
public sealed class CatalogRepository
{
    private readonly Supabase.Client _db;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IReadOnlyList<Destination>? _destinations;   // destinations never change in the app: load once

    public CatalogRepository(Supabase.Client db) => _db = db;

    // ------------------------------------------------------------------ READ

    /// <summary>destinations + seasonal_highlights (2 requests, cached after the first call).</summary>
    public async Task<IReadOnlyList<Destination>> GetDestinationsAsync()
    {
        if (_destinations is not null) return _destinations;
        await _gate.WaitAsync();
        try
        {
            if (_destinations is not null) return _destinations;

            var dRes = await _db.From<DestinationRow>().Order(d => d.Id, Ordering.Ascending).Range(0, 99).Get();
            var hRes = await _db.From<SeasonalHighlightRow>().Order(h => h.Id, Ordering.Ascending).Range(0, 499).Get();
            var highlights = hRes.Models.ToLookup(h => h.DestinationId);

            _destinations = dRes.Models.Select(d => new Destination(
                (int)d.Id, d.Name, d.JpName, Enum.Parse<Region>(d.Region), d.Prefecture, d.Blurb, d.MapX, d.MapY,
                highlights[d.Id].Select(h => new SeasonalHighlight(h.StartMonth, h.EndMonth, h.Title, h.Note)).ToList(),
                d.LabelLeft, d.PhotoUrl, d.PhotoAlt)).ToList();
            return _destinations;
        }
        finally { _gate.Release(); }
    }

    /// <summary>experiences (1 request). Seat counts are always fresh from the database.</summary>
    public async Task<List<Experience>> GetExperiencesAsync(IReadOnlyList<Destination> destinations)
    {
        var res = await _db.From<ExperienceRow>().Order(e => e.Id, Ordering.Ascending).Range(0, 199).Get();
        var byId = destinations.ToDictionary(d => d.Id);
        return res.Models.Select(r => ToExperience(r, byId[(int)r.DestinationId])).ToList();
    }

    /// <summary>The 10 newest bookings that are not soft-deleted, with their lines (2 requests).</summary>
    public async Task<List<Booking>> GetRecentBookingsAsync()
    {
        var bRes = await _db.From<BookingRow>()
            .Where(b => b.DeletedAt == null)              // soft-deleted rows stay in the table but are skipped here
            .Order("created_at", Ordering.Descending)
            .Range(0, 9)
            .Get();
        if (bRes.Models.Count == 0) return new List<Booking>();

        var ids = bRes.Models.Select(b => (object)b.Id).ToList();
        var lRes = await _db.From<BookingLineRow>()
            .Filter("booking_id", Operator.In, ids)       // one request for all lines, not one per booking
            .Order(l => l.Id, Ordering.Ascending)
            .Get();
        var lines = lRes.Models.ToLookup(l => l.BookingId);

        return bRes.Models.Select(b => new Booking(
            b.Id, b.Reference, b.LeaderName, b.Email, b.Travelers, b.Departure,
            Enum.Parse<Pace>(b.Pace), b.Notes,
            lines[b.Id].Select(l => new BookingLine(l.Name, l.Place, l.Code, Enum.Parse<Region>(l.Region),
                l.Days, l.Cost, (int)l.ExperienceId)))).ToList();
    }

    // ------------------------------------------------------------------ CREATE

    /// <summary>
    /// Saves a booking: reserves seats on every stop (UPDATE), inserts the booking (INSERT) and its
    /// price lines (INSERT). Returns null if another visitor took the seats first; in that case
    /// nothing is left half-done. If a later step fails, seats are given back and the error is rethrown.
    /// </summary>
    public async Task<Booking?> CreateBookingAsync(string reference, string leader, string email, int travelers,
        DateTime departure, Pace pace, string notes, IReadOnlyList<Experience> stops)
    {
        // 1. Reserve seats. "Where booked == old" means the update only works if nobody else changed the row.
        var reserved = new List<(long Id, int Old)>();
        foreach (var stop in stops)
        {
            long id = stop.Id;
            int old = stop.Booked;
            int next = old + travelers;
            var res = await _db.From<ExperienceRow>()
                .Where(x => x.Id == id)
                .Where(x => x.Booked == old)
                .Set(x => x.Booked, next)
                .Update();
            if (res.Models.Count == 0)
            {
                await GiveSeatsBackAsync(reserved, travelers);
                return null;
            }
            reserved.Add((id, old));
        }

        var booking = new Booking(reference, leader, email, travelers, departure, pace, stops, notes);
        try
        {
            // 2. The booking row.
            var ins = await _db.From<BookingRow>().Insert(new BookingRow
            {
                Reference = booking.Reference,
                LeaderName = booking.LeaderName,
                Email = booking.Email,
                Travelers = booking.Travelers,
                Departure = booking.Departure.Date,
                Pace = booking.Pace.ToString(),
                Notes = booking.Notes
            });
            booking.Id = ins.Models.First().Id;

            // 3. Its price lines, all in one request.
            var lineRows = booking.Lines.Select(l => new BookingLineRow
            {
                BookingId = booking.Id,
                ExperienceId = l.ExperienceId,
                Name = l.Name,
                Place = l.Place,
                Code = l.Code,
                Region = l.Region.ToString(),
                Days = l.Days,
                Cost = l.Cost
            }).ToList();
            await _db.From<BookingLineRow>().Insert(lineRows);
            return booking;
        }
        catch
        {
            if (booking.Id != 0) await SoftDeleteBookingAsync(booking.Id);
            await GiveSeatsBackAsync(reserved, travelers);
            throw;
        }
    }

    // ------------------------------------------------------------------ UPDATE

    /// <summary>Moves the departure date of one booking.</summary>
    public async Task RescheduleAsync(long bookingId, DateTime departure) =>
        await _db.From<BookingRow>()
            .Where(x => x.Id == bookingId)
            .Set(x => x.Departure, departure.Date)
            .Update();

    /// <summary>Changes the seat count of one experience, only if it still holds the value we last read.</summary>
    public async Task SetBookedAsync(long experienceId, int oldBooked, int newBooked) =>
        await _db.From<ExperienceRow>()
            .Where(x => x.Id == experienceId)
            .Where(x => x.Booked == oldBooked)
            .Set(x => x.Booked, newBooked)
            .Update();

    // ------------------------------------------------------------------ DELETE (soft)

    /// <summary>Soft delete: fills deleted_at. The booking and its lines stay in the tables as history.</summary>
    public async Task SoftDeleteBookingAsync(long bookingId) =>
        await _db.From<BookingRow>()
            .Where(x => x.Id == bookingId)
            .Set(x => x.DeletedAt, DateTime.UtcNow)
            .Update();

    // ------------------------------------------------------------------ helpers

    private async Task GiveSeatsBackAsync(List<(long Id, int Old)> reserved, int travelers)
    {
        foreach (var (id, old) in reserved)
            await SetBookedAsync(id, old + travelers, old);
    }

    /// <summary>Picks the right Experience subclass from the "kind" column.</summary>
    private static Experience ToExperience(ExperienceRow r, Destination d)
    {
        int id = (int)r.Id;
        return r.Kind switch
        {
            "GuidedTour" => new GuidedTour(id, r.Name, d, r.Days, r.BasePrice, r.Capacity, r.Booked,
                r.Description, r.Guide ?? ""),
            "RailJourney" => new RailJourney(id, r.Name, d, r.Days, r.BasePrice, r.Capacity, r.Booked,
                r.Description, r.PassName ?? "Rail Pass", r.ReservationFee ?? 0),
            "HomeStay" => new HomeStay(id, r.Name, d, r.Days, r.BasePrice, r.Capacity, r.Booked,
                r.Description, r.CleaningFee ?? 0),
            "Workshop" => new CraftWorkshop(id, r.Name, d, r.Days, r.BasePrice, r.Capacity, r.Booked,
                r.Description, r.StudioFee ?? 0, r.Craft ?? "Craft"),
            _ => throw new InvalidOperationException($"Unknown experience kind '{r.Kind}' on experiences.id = {r.Id}.")
        };
    }
}