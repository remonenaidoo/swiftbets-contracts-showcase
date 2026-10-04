namespace SwiftBets.Contracts.Offer;

/// <summary>
/// A market id is <c>{fixtureId}-{suffix}</c>; the suffix names the market type, so settlement can resolve a leg from its
/// market and selection ids alone. Suffixes never contain a dash.
/// </summary>
public static class MarketIds
{
    private static readonly Dictionary<MarketType, string> Suffixes = new()
    {
        [MarketType.MatchResult] = "1x2",
        [MarketType.TotalGoalsOverUnder25] = "ou25",
        [MarketType.MatchWinner] = "mw",
        [MarketType.SetHandicap] = "sethcp",
        [MarketType.TotalGames] = "games",
        [MarketType.Handicap] = "hcp",
        [MarketType.TotalPoints] = "pts",
        [MarketType.TopBatter] = "topbat",
        [MarketType.CompetitionWinner] = "winner",
    };

    public static string Suffix(MarketType type) => Suffixes[type];

    public static string For(string fixtureId, MarketType type) => $"{fixtureId}-{Suffixes[type]}";

    /// <summary>The market type a market id names, or null when its suffix is unknown.</summary>
    public static MarketType? TypeOf(string marketId)
    {
        var suffix = marketId[(marketId.LastIndexOf('-') + 1)..];
        foreach (var (type, known) in Suffixes)
        {
            if (known == suffix)
            {
                return type;
            }
        }

        return null;
    }
}
