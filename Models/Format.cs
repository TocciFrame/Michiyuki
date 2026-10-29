using System.Globalization;

namespace Michiyuki.Models;

public static class Format
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public static string Yen(this decimal value) =>
        (value < 0 ? "-" : "") + "¥" + Math.Abs(value).ToString("N0", Inv);

    public static string Day(this DateTime date) => date.ToString("ddd d MMM yyyy", Inv);
}
