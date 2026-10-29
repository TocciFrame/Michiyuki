namespace Michiyuki.Models;

/// <summary>The regional travel lines used by the site.</summary>
public enum Region { Hokkaido, Tohoku, Kanto, Chubu, Kansai, Shikoku, Kyushu, Okinawa }

public enum ExperienceKind { GuidedTour, RailJourney, HomeStay, Workshop }

public enum SortMode { Route, PriceLow, PriceHigh, Longest, SeatsLeft }

public enum Pace { Slow, Steady, Brisk }

public static class EnumLabels
{
    public static string Label(this Region r) => r.ToString();

    /// <summary>One-letter line code used on station signs (H01, K03...).</summary>
    public static string Code(this Region r) => r switch
    {
        Region.Hokkaido => "H",
        Region.Tohoku => "T",
        Region.Kanto => "K",
        Region.Chubu => "C",
        Region.Kansai => "W",
        Region.Shikoku => "S",
        Region.Okinawa => "O",
        _ => "Q"
    };

    public static string Label(this ExperienceKind k) => k switch
    {
        ExperienceKind.GuidedTour => "Guided walk",
        ExperienceKind.RailJourney => "Rail journey",
        ExperienceKind.HomeStay => "Stay",
        _ => "Workshop"
    };

    public static string Label(this SortMode s) => s switch
    {
        SortMode.Route => "North to south",
        SortMode.PriceLow => "Price, low to high",
        SortMode.PriceHigh => "Price, high to low",
        SortMode.Longest => "Longest first",
        _ => "Most seats left"
    };

    public static string Label(this Pace p) => p switch
    {
        Pace.Slow => "Slow: one thing a day",
        Pace.Steady => "Steady: a morning and an afternoon",
        _ => "Brisk: as much as fits"
    };
}
