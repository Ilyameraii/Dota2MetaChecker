using System.Globalization;

namespace Services.Formatting.Extensions;

public static class DeltaExtensions
{
    public static string FormatDelta(this double delta)
    {
        delta = Math.Round(delta * 100, 2);
        var sign = delta switch
        {
            < 0 => "-",
            >= 0 => "+",
            _ => string.Empty
        };
        return $"{sign}{Math.Abs(delta).ToString("F2", CultureInfo.InvariantCulture)}";
    }
}