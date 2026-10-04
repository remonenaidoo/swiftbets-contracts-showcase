namespace SwiftBets.Contracts.Promotions;

/// <summary>
/// The accumulator boost: a percentage of the winnings, by number of legs, paid at settlement when the bet wins. Only a
/// plain accumulator qualifies and every leg must be priced at least <paramref name="MinLegOdds"/>. The tiers map a leg
/// count to a percentage; a bet takes the tier of the largest leg count it reaches.
/// </summary>
public sealed record AccaBoost(IReadOnlyDictionary<int, decimal> Tiers, decimal MinLegOdds)
{
    public const int MinLegs = 2;
    public const int MaxLegs = 20;
    public const decimal DefaultMinLegOdds = 1.2m;

    public static AccaBoost None { get; } = new(new Dictionary<int, decimal>(), DefaultMinLegOdds);

    /// <summary>The percentage this accumulator earns, or 0.</summary>
    public decimal PercentFor(IReadOnlyList<decimal> legOdds)
    {
        ArgumentNullException.ThrowIfNull(legOdds);
        if (legOdds.Count < MinLegs || legOdds.Any(o => o < MinLegOdds))
        {
            return 0m;
        }

        return Tiers.Where(t => t.Key <= legOdds.Count).OrderByDescending(t => t.Key).Select(t => t.Value).FirstOrDefault();
    }

    /// <summary>The bonus on a winning return: the percentage of the winnings above the stake, rounded down to the minor unit.</summary>
    public static long Bonus(long stake, long returned, decimal percent) =>
        returned <= stake || percent <= 0 ? 0 : (long)decimal.Floor((returned - stake) * percent / 100m);
}
