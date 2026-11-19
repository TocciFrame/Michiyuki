using Michiyuki.Data;

namespace Michiyuki.Models;

/// <summary>
/// Scoped per browser circuit. Loads the catalog from Supabase, owns the traveler's route and keeps
/// the bookings made. All route rules (no duplicates, seat checks, totals) live here, not in the pages;
/// all database access goes through <see cref="CatalogRepository"/>.
/// </summary>
public sealed class TripPlanner
{
    public const int MaxTravelers = 12;

    private readonly CatalogRepository _repo;
    private readonly List<Experience> _route = new();
    private IReadOnlyList<Destination> _destinations = Array.Empty<Destination>();
    private List<Experience> _catalog = new();
    private List<Booking> _bookings = new();
    private int _travelers = 2;
    private Task? _loading;

    public TripPlanner(CatalogRepository repo) => _repo = repo;

    public event Action? Changed;

    public bool IsLoaded { get; private set; }
    /// <summary>Why the last load failed (wrong key, paused project, missing table...), or null.</summary>
    public string? LoadError { get; private set; }

    public IReadOnlyList<Destination> Destinations => _destinations;
    public IReadOnlyList<Experience> Catalog => _catalog;
    public IReadOnlyList<Experience> Route => _route;
    public IReadOnlyList<Booking> Bookings => _bookings;

    public int Travelers
    {
        get => _travelers;
        set
        {
            _travelers = Math.Clamp(value, 1, MaxTravelers);
            Changed?.Invoke();
        }
    }

    public int RouteDays => _route.Sum(e => e.Days);
    public decimal RouteTotal => _route.Sum(e => e.CalculateCost(_travelers));
    public decimal TotalFor(int travelers) => _route.Sum(e => e.CalculateCost(travelers));

    public bool InRoute(Experience e) => _route.Any(r => r.Id == e.Id);

    // ------------------------------------------------------------------ loading

    /// <summary>Loads destinations, experiences and recent bookings once per circuit. Never throws; check IsLoaded.</summary>
    public Task EnsureLoadedAsync() => _loading ??= LoadAsync();

    private async Task LoadAsync()
    {
        try
        {
            _destinations = await _repo.GetDestinationsAsync();
            _catalog = await _repo.GetExperiencesAsync(_destinations);
            _bookings = await _repo.GetRecentBookingsAsync();
            LoadError = null;
            IsLoaded = true;
        }
        catch (Exception ex)
        {
            LoadError = ex.Message;
            IsLoaded = false;
            _loading = null;   // let the next page visit try again
        }
    }

    /// <summary>Swaps in fresh experiences (new seat counts) and points the route at the new objects.</summary>
    private void ReplaceCatalog(List<Experience> fresh)
    {
        _catalog = fresh;
        var byId = fresh.ToDictionary(e => e.Id);
        for (var i = _route.Count - 1; i >= 0; i--)
        {
            if (byId.TryGetValue(_route[i].Id, out var f)) _route[i] = f;
            else _route.RemoveAt(i);
        }
    }

    // ------------------------------------------------------------------ route (in memory)

    public bool Add(Experience e)
    {
        if (e.IsFull || InRoute(e)) return false;
        _route.Add(e);
        Changed?.Invoke();
        return true;
    }

    public void Remove(Experience e)
    {
        if (_route.RemoveAll(r => r.Id == e.Id) > 0) Changed?.Invoke();
    }

    public void Clear()
    {
        _route.Clear();
        Changed?.Invoke();
    }

    /// <summary>Stops that cannot seat this many travelers.</summary>
    public IEnumerable<Experience> Unavailable(int travelers) => _route.Where(e => e.SeatsLeft < travelers);

    // ------------------------------------------------------------------ bookings (database)

    /// <summary>
    /// CREATE. Saves the booking and reserves seats in Supabase. Returns null if any stop is short
    /// (including seats taken by another visitor a moment ago). Database errors are thrown.
    /// </summary>
    public async Task<Booking?> ConfirmAsync(string leader, string email, int travelers, DateTime departure, Pace pace, string notes)
    {
        if (_route.Count == 0 || Unavailable(travelers).Any()) return null;

        var reference = $"MCY-{departure:MMdd}-{Random.Shared.Next(1000, 9999)}";
        var booking = await _repo.CreateBookingAsync(reference, leader, email, travelers, departure, pace, notes, _route.ToList());
        if (booking is null)
        {
            ReplaceCatalog(await _repo.GetExperiencesAsync(_destinations));   // show the real seat counts
            Changed?.Invoke();
            return null;
        }

        foreach (var stop in _route) stop.TryReserve(travelers);   // mirror what the database now holds
        _bookings.Insert(0, booking);
        _route.Clear();
        _travelers = travelers;
        Changed?.Invoke();
        return booking;
    }

    /// <summary>UPDATE. Moves the departure date.</summary>
    public async Task RescheduleAsync(Booking booking, DateTime departure)
    {
        await _repo.RescheduleAsync(booking.Id, departure);
        booking.Reschedule(departure);
        Changed?.Invoke();
    }

    /// <summary>DELETE (soft). Marks the booking as deleted and gives its seats back.</summary>
    public async Task CancelAsync(Booking booking)
    {
        await _repo.SoftDeleteBookingAsync(booking.Id);

        // Seat counts may have moved since we loaded them, so read them again before giving seats back.
        var fresh = await _repo.GetExperiencesAsync(_destinations);
        var byId = fresh.ToDictionary(e => e.Id);
        foreach (var line in booking.Lines)
        {
            if (!byId.TryGetValue(line.ExperienceId, out var exp)) continue;
            var old = exp.Booked;
            exp.Release(booking.Travelers);
            await _repo.SetBookedAsync(exp.Id, old, exp.Booked);
        }

        ReplaceCatalog(fresh);
        _bookings.RemoveAll(b => b.Id == booking.Id);
        Changed?.Invoke();
    }
}