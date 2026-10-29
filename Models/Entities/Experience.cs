namespace Michiyuki.Models;

/// <summary>One priced line in a cost breakdown.</summary>
public sealed record CostLine(string Label, decimal Amount);

/// <summary>Anything with a limited number of seats.</summary>
public interface IBookable
{
    int Capacity { get; }
    int SeatsLeft { get; }
    bool IsFull { get; }
    bool TryReserve(int seats);
}

/// <summary>
/// Base type for everything a traveler can add to a route. Subclasses decide how they are
/// priced (Breakdown) and how they describe themselves (Kind, Notes); the base class owns
/// identity, seat tracking and the total (sum of the breakdown).
/// </summary>
public abstract class Experience : IBookable
{
    protected Experience(int id, string name, Destination destination, int days,
        decimal basePrice, int capacity, int booked, string description)
    {
        Id = id;
        Name = name;
        Destination = destination;
        Days = days;
        BasePrice = basePrice;
        Capacity = capacity;
        Booked = booked;
        Description = description;
    }

    public int Id { get; }
    public string Name { get; }
    public Destination Destination { get; }
    public int Days { get; }
    public string Description { get; }
    public int Capacity { get; }
    public int Booked { get; private set; }
    protected decimal BasePrice { get; }

    public Region Region => Destination.Region;
    public int SeatsLeft => Capacity - Booked;
    public bool IsFull => SeatsLeft <= 0;
    public bool IsAlmostFull => !IsFull && SeatsLeft <= 3;

    public abstract ExperienceKind Kind { get; }
    /// <summary>What the price covers, in one short sentence.</summary>
    public abstract string Notes { get; }
    public abstract IEnumerable<CostLine> Breakdown(int travelers);

    public decimal CalculateCost(int travelers) => Breakdown(Normalize(travelers)).Sum(l => l.Amount);
    public decimal CostPerPerson(int travelers) => CalculateCost(travelers) / Normalize(travelers);

    public bool TryReserve(int seats)
    {
        if (seats <= 0 || seats > SeatsLeft) return false;
        Booked += seats;
        return true;
    }

    protected static int Normalize(int travelers) => Math.Max(1, travelers);
    protected static decimal Yen(decimal value) => Math.Round(value, 0);
}

/// <summary>Priced per person, with a group discount from four travelers.</summary>
public sealed class GuidedTour : Experience
{
    public const int DiscountFrom = 4;
    public const decimal DiscountRate = 0.10m;

    public GuidedTour(int id, string name, Destination destination, int days, decimal pricePerPerson,
        int capacity, int booked, string description, string guide)
        : base(id, name, destination, days, pricePerPerson, capacity, booked, description) => Guide = guide;

    public string Guide { get; }
    public override ExperienceKind Kind => ExperienceKind.GuidedTour;
    public override string Notes => $"Led by {Guide}. Groups of {DiscountFrom}+ save {DiscountRate:P0}.";

    public override IEnumerable<CostLine> Breakdown(int travelers)
    {
        var gross = BasePrice * travelers;
        yield return new CostLine($"Guided walk x {travelers}", gross);
        if (travelers >= DiscountFrom)
            yield return new CostLine($"Group discount ({DiscountRate:P0})", -Yen(gross * DiscountRate));
    }
}

/// <summary>A rail pass per person plus one shared seat-reservation fee.</summary>
public sealed class RailJourney : Experience
{
    public RailJourney(int id, string name, Destination destination, int days, decimal passPrice,
        int capacity, int booked, string description, string passName, decimal reservationFee)
        : base(id, name, destination, days, passPrice, capacity, booked, description)
    {
        PassName = passName;
        ReservationFee = reservationFee;
    }

    public string PassName { get; }
    public decimal ReservationFee { get; }
    public override ExperienceKind Kind => ExperienceKind.RailJourney;
    public override string Notes => $"Includes the {PassName} and reserved seats for the whole party.";

    public override IEnumerable<CostLine> Breakdown(int travelers)
    {
        yield return new CostLine($"{PassName} x {travelers}", BasePrice * travelers);
        yield return new CostLine("Seat reservations (shared)", ReservationFee);
    }
}

/// <summary>Priced per room per night; a room sleeps two.</summary>
public sealed class HomeStay : Experience
{
    public const int GuestsPerRoom = 2;

    public HomeStay(int id, string name, Destination destination, int nights, decimal nightlyRate,
        int capacity, int booked, string description, decimal cleaningFee)
        : base(id, name, destination, nights, nightlyRate, capacity, booked, description) => CleaningFee = cleaningFee;

    public decimal CleaningFee { get; }
    public override ExperienceKind Kind => ExperienceKind.HomeStay;
    public override string Notes => $"Each room sleeps {GuestsPerRoom}. Breakfast is included.";

    public static int RoomsFor(int travelers) => (int)Math.Ceiling(travelers / (double)GuestsPerRoom);

    public override IEnumerable<CostLine> Breakdown(int travelers)
    {
        var rooms = RoomsFor(travelers);
        yield return new CostLine($"{rooms} room(s) x {Days} night(s)", BasePrice * rooms * Days);
        yield return new CostLine("Cleaning fee", CleaningFee);
    }
}

/// <summary>Materials per person plus a shared studio fee.</summary>
public sealed class CraftWorkshop : Experience
{
    public CraftWorkshop(int id, string name, Destination destination, int days, decimal materialsPerPerson,
        int capacity, int booked, string description, decimal studioFee, string craft)
        : base(id, name, destination, days, materialsPerPerson, capacity, booked, description)
    {
        StudioFee = studioFee;
        Craft = craft;
    }

    public decimal StudioFee { get; }
    public string Craft { get; }
    public override ExperienceKind Kind => ExperienceKind.Workshop;
    public override string Notes => $"Hands-on {Craft.ToLowerInvariant()}. You take your piece home.";

    public override IEnumerable<CostLine> Breakdown(int travelers)
    {
        yield return new CostLine($"Materials x {travelers}", BasePrice * travelers);
        yield return new CostLine("Studio fee (shared)", StudioFee);
    }
}
