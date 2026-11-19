namespace Michiyuki.Models;

public sealed record BookingLine(string Name, string Place, string Code, Region Region, int Days, decimal Cost, int ExperienceId = 0);

/// <summary>A confirmed trip. Takes a snapshot of prices at the moment of booking.</summary>
public sealed class Booking
{
    /// <summary>New booking built from the stops on the route (not saved yet, so Id is 0).</summary>
    public Booking(string reference, string leaderName, string email, int travelers,
        DateTime departure, Pace pace, IEnumerable<Experience> stops, string notes = "")
    {
        Reference = reference;
        LeaderName = leaderName;
        Email = email;
        Travelers = travelers;
        Departure = departure;
        Pace = pace;
        Notes = notes;
        Lines = stops.Select(s => new BookingLine(s.Name, s.Destination.Name, s.Destination.Code,
            s.Region, s.Days, s.CalculateCost(travelers), s.Id)).ToList();
    }

    /// <summary>Booking read back from the database, with its saved price lines.</summary>
    public Booking(long id, string reference, string leaderName, string email, int travelers,
        DateTime departure, Pace pace, string notes, IEnumerable<BookingLine> lines)
    {
        Id = id;
        Reference = reference;
        LeaderName = leaderName;
        Email = email;
        Travelers = travelers;
        Departure = departure;
        Pace = pace;
        Notes = notes;
        Lines = lines.ToList();
    }

    /// <summary>bookings.id. 0 until the booking is saved.</summary>
    public long Id { get; internal set; }
    public string Reference { get; }
    public string LeaderName { get; }
    public string Email { get; }
    public int Travelers { get; }
    public DateTime Departure { get; private set; }
    public Pace Pace { get; }
    public string Notes { get; }
    public IReadOnlyList<BookingLine> Lines { get; }

    public decimal Total => Lines.Sum(l => l.Cost);
    public int TotalDays => Lines.Sum(l => l.Days);
    public DateTime Return => Departure.AddDays(Math.Max(0, TotalDays - 1));

    public void Reschedule(DateTime departure) => Departure = departure.Date;
}