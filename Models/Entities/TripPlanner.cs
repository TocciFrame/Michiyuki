using Michiyuki.Data;

namespace Michiyuki.Models;

/// <summary>
/// Scoped per browser circuit. Owns the catalog, the traveler's route and the bookings made.
/// All route rules (no duplicates, seat checks, totals) live here, not in the pages.
/// </summary>
public sealed class TripPlanner
{
    public const int MaxTravelers = 12;

    private readonly List<Experience> _route = new();
    private readonly List<Booking> _bookings = new();
    private int _travelers = 2;

    public TripPlanner() => Catalog = SampleData.CreateExperiences();

    public event Action? Changed;

    public IReadOnlyList<Experience> Catalog { get; }
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

    /// <summary>Reserves seats on every stop and returns the booking, or null if any stop is short.</summary>
    public Booking? Confirm(string leader, string email, int travelers, DateTime departure, Pace pace)
    {
        if (_route.Count == 0 || Unavailable(travelers).Any()) return null;
        foreach (var stop in _route) stop.TryReserve(travelers);

        var reference = $"MCY-{departure:MMdd}-{Random.Shared.Next(1000, 9999)}";
        var booking = new Booking(reference, leader, email, travelers, departure, pace, _route);
        _bookings.Add(booking);
        _route.Clear();
        _travelers = travelers;
        Changed?.Invoke();
        return booking;
    }
}
