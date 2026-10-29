namespace Michiyuki.Models;

public sealed record BookingLine(string Name, string Place, string Code, Region Region, int Days, decimal Cost);

/// <summary>A confirmed trip. Takes a snapshot of prices at the moment of booking.</summary>
public sealed class Booking
{
    public Booking(string reference, string leaderName, string email, int travelers,
        DateTime departure, Pace pace, IEnumerable<Experience> stops)
    {
        Reference = reference;
        LeaderName = leaderName;
        Email = email;
        Travelers = travelers;
        Departure = departure;
        Pace = pace;
        Lines = stops.Select(s => new BookingLine(s.Name, s.Destination.Name, s.Destination.Code,
            s.Region, s.Days, s.CalculateCost(travelers))).ToList();
    }

    public string Reference { get; }
    public string LeaderName { get; }
    public string Email { get; }
    public int Travelers { get; }
    public DateTime Departure { get; }
    public Pace Pace { get; }
    public IReadOnlyList<BookingLine> Lines { get; }

    public decimal Total => Lines.Sum(l => l.Cost);
    public int TotalDays => Lines.Sum(l => l.Days);
    public DateTime Return => Departure.AddDays(Math.Max(0, TotalDays - 1));
}
