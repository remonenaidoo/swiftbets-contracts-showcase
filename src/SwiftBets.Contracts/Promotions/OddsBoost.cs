using System.Globalization;

namespace SwiftBets.Contracts.Promotions;

/// <summary>
/// A trader's boosted price on one selection between <see cref="From"/> and <see cref="Until"/>, with the most one
/// customer may stake on it across all their bets in minor units. Stored as "odds;from;until;maxStake" with ISO times.
/// </summary>
public sealed record OddsBoost(decimal Odds, DateTimeOffset From, DateTimeOffset Until, long MaxStakePerCustomer)
{
    public bool IsActive(DateTimeOffset now) => now >= From && now < Until;

    public string Format() => string.Create(CultureInfo.InvariantCulture,
        $"{Odds:0.00};{From.UtcDateTime:yyyy-MM-ddTHH:mm:ssZ};{Until.UtcDateTime:yyyy-MM-ddTHH:mm:ssZ};{MaxStakePerCustomer}");

    public static OddsBoost? Parse(string? value)
    {
        var parts = value?.Split(';');
        if (parts is not { Length: 4 }
            || !decimal.TryParse(parts[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var odds)
            || !DateTimeOffset.TryParse(parts[1], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var from)
            || !DateTimeOffset.TryParse(parts[2], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var until)
            || !long.TryParse(parts[3], NumberStyles.None, CultureInfo.InvariantCulture, out var maxStake))
        {
            return null;
        }

        return odds > 1m && until > from && maxStake > 0 ? new OddsBoost(odds, from, until, maxStake) : null;
    }
}
