namespace Michiyuki.Models;

/// <summary>A named seasonal event at a destination. Ranges may wrap the new year (Nov to Mar).</summary>
public sealed class SeasonalHighlight
{
    public static readonly string[] MonthNames =
        { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

    public SeasonalHighlight(int startMonth, int endMonth, string title, string note)
    {
        if (startMonth is < 1 or > 12 || endMonth is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(startMonth), "Months must be 1-12.");
        StartMonth = startMonth;
        EndMonth = endMonth;
        Title = title;
        Note = note;
    }

    public int StartMonth { get; }
    public int EndMonth { get; }
    public string Title { get; }
    public string Note { get; }

    public bool Covers(int month) =>
        StartMonth <= EndMonth
            ? month >= StartMonth && month <= EndMonth
            : month >= StartMonth || month <= EndMonth;

    public string RangeLabel =>
        StartMonth == EndMonth
            ? MonthNames[StartMonth - 1]
            : $"{MonthNames[StartMonth - 1]} to {MonthNames[EndMonth - 1]}";
}
