using System.Globalization;
using System.Text.RegularExpressions;

namespace SwiftBets.Contracts.Config;

/// <summary>How placement takes bets: everything, pre-match only, or nothing.</summary>
public enum PlacementMode
{
    Open = 1,
    PreMatchOnly = 2,
    Closed = 3,
}

/// <summary>
/// The settings services read, and the one parser and validator for each, so the config service refuses what a consumer
/// could not read. A key a consumer does not find means its built-in default.
/// </summary>
public static partial class ConfigKeys
{
    /// <summary>"on" stops all placement at once; "off" (or absent) lets the mode decide.</summary>
    public const string PlacementKillSwitch = "placement.kill-switch";

    /// <summary>open, preMatchOnly or closed.</summary>
    public const string PlacementMode = "placement.mode";

    /// <summary>Seconds an in-play bet waits before acceptance, per sport, with a default.</summary>
    public static string LiveDelaySeconds(string sport) => $"placement.live-delay-seconds.{sport}";

    public const string LiveDelayDefault = "placement.live-delay-seconds.default";

    /// <summary>Largest stake per coupon, in minor units of the currency.</summary>
    public static string MaxStake(string currency) => $"limits.max-stake.{currency}";

    /// <summary>Largest potential payout per coupon, in minor units of the currency.</summary>
    public static string MaxPayout(string currency) => $"limits.max-payout.{currency}";

    /// <summary>A feature flag: "true" or "false".</summary>
    public static string Flag(string name) => $"flags.{name}";

    /// <summary>"false" turns the bet builder off for one competition; absent means on.</summary>
    public static string BetBuilderCompetition(string competitionId) => $"betbuilder.competition.{competitionId}";

    /// <summary>The correlation factor of a pair of selections, 0.1 to 3; see <see cref="Offer.BetBuilderPricing"/>.</summary>
    public static string BetBuilderCorrelation(string selectionA, string selectionB) => $"betbuilder.correlation.{Offer.BetBuilderPricing.PairKey(selectionA, selectionB)}";

    public const string BetBuilderCorrelationPrefix = "betbuilder.correlation.";

    /// <summary>The bet builder margin in percent, 0 to 30.</summary>
    public const string BetBuilderMargin = "betbuilder.margin-percent";

    /// <summary>Largest potential payout of a coupon holding a bet builder, in minor units of the currency.</summary>
    public static string BetBuilderMaxPayout(string currency) => $"betbuilder.max-payout.{currency}";

    /// <summary>The accumulator boost percentage, 0 to 100, from this many legs up.</summary>
    public static string AccaBoostTier(int legs) => string.Create(CultureInfo.InvariantCulture, $"boosts.acca.legs.{legs}");

    public const string AccaBoostTierPrefix = "boosts.acca.legs.";

    /// <summary>The lowest price every leg needs for an accumulator boost.</summary>
    public const string AccaBoostMinLegOdds = "boosts.acca.min-leg-odds";

    /// <summary>A boosted price on one selection (see <see cref="Promotions.OddsBoost"/>), or "off".</summary>
    public static string OddsBoost(string marketId, string selectionId) => $"boosts.odds.{marketId}.{selectionId}";

    public const string OddsBoostPrefix = "boosts.odds.";

    /// <summary>Whether a competition offers the bet builder.</summary>
    public static bool IsBetBuilderOn(IReadOnlyDictionary<string, string> settings, string competitionId) =>
        !string.Equals(settings.GetValueOrDefault(BetBuilderCompetition(competitionId)), "false", StringComparison.Ordinal);

    /// <summary>Configured correlation factors by pair key; pairs not set fall back to the built-in table.</summary>
    public static IReadOnlyDictionary<string, decimal> BetBuilderFactors(IReadOnlyDictionary<string, string> settings) =>
        settings.Where(s => s.Key.StartsWith(BetBuilderCorrelationPrefix, StringComparison.Ordinal))
            .Select(s => (Key: s.Key[BetBuilderCorrelationPrefix.Length..], Factor: Offer.BetBuilderPricing.ParseFactor(s.Value)))
            .Where(s => s.Factor is not null)
            .ToDictionary(s => s.Key, s => s.Factor!.Value, StringComparer.Ordinal);

    public static decimal BetBuilderMarginPercent(IReadOnlyDictionary<string, string> settings) =>
        ParsePercent(settings.GetValueOrDefault(BetBuilderMargin), 30m) ?? Offer.BetBuilderPricing.DefaultMarginPercent;

    /// <summary>The accumulator boost in force; no tiers means no boost.</summary>
    public static Promotions.AccaBoost AccaBoost(IReadOnlyDictionary<string, string> settings)
    {
        var tiers = settings.Where(s => s.Key.StartsWith(AccaBoostTierPrefix, StringComparison.Ordinal))
            .Select(s => (Legs: int.TryParse(s.Key[AccaBoostTierPrefix.Length..], NumberStyles.None, CultureInfo.InvariantCulture, out var legs) ? legs : 0, Percent: ParsePercent(s.Value, 100m)))
            .Where(t => t.Legs >= Promotions.AccaBoost.MinLegs && t.Legs <= Promotions.AccaBoost.MaxLegs && t.Percent is > 0)
            .ToDictionary(t => t.Legs, t => t.Percent!.Value);
        return new Promotions.AccaBoost(tiers, ParseOdds(settings.GetValueOrDefault(AccaBoostMinLegOdds)) ?? Promotions.AccaBoost.DefaultMinLegOdds);
    }

    /// <summary>The boost on this selection if one is set and in its window now.</summary>
    public static Promotions.OddsBoost? ActiveOddsBoost(IReadOnlyDictionary<string, string> settings, string marketId, string selectionId, DateTimeOffset now) =>
        Promotions.OddsBoost.Parse(settings.GetValueOrDefault(OddsBoost(marketId, selectionId))) is { } boost && boost.IsActive(now) ? boost : null;

    public static bool IsKillSwitchOn(string? value) => string.Equals(value, "on", StringComparison.Ordinal);

    public static PlacementMode ParseMode(string? value) => value switch
    {
        "preMatchOnly" => Config.PlacementMode.PreMatchOnly,
        "closed" => Config.PlacementMode.Closed,
        _ => Config.PlacementMode.Open,
    };

    public static long? ParseMinorUnits(string? value) =>
        long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var amount) ? amount : null;

    /// <summary>Why this value is not acceptable for this key, or null when it is.</summary>
    public static string? Validate(string key, string value)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);
        if (key == PlacementKillSwitch)
        {
            return value is "on" or "off" ? null : "The kill switch is on or off.";
        }

        if (key == PlacementMode)
        {
            return value is "open" or "preMatchOnly" or "closed" ? null : "The mode is open, preMatchOnly or closed.";
        }

        if (LiveDelayKey().IsMatch(key))
        {
            return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var seconds) && seconds <= 30 ? null : "A live delay is 0 to 30 seconds.";
        }

        if (LimitKey().IsMatch(key))
        {
            return ParseMinorUnits(value) is > 0 ? null : "A limit is a whole number of minor units above zero.";
        }

        if (BetBuilderCompetitionKey().IsMatch(key))
        {
            return value is "true" or "false" ? null : "The bet builder is true or false for a competition.";
        }

        if (BetBuilderCorrelationKey().IsMatch(key))
        {
            var pair = key[BetBuilderCorrelationPrefix.Length..].Split('.');
            return pair[0] == pair[1] || Offer.BetBuilderPricing.PairKey(pair[0], pair[1]) != $"{pair[0]}.{pair[1]}"
                ? "A pair is two different selections in alphabetical order."
                : Offer.BetBuilderPricing.ParseFactor(value) is null ? "A correlation factor is 0.1 to 3." : null;
        }

        if (key == BetBuilderMargin)
        {
            return ParsePercent(value, 30m) is null ? "The margin is 0 to 30 percent." : null;
        }

        if (BetBuilderMaxPayoutKey().IsMatch(key))
        {
            return ParseMinorUnits(value) is > 0 ? null : "A limit is a whole number of minor units above zero.";
        }

        if (AccaTierKey().IsMatch(key))
        {
            var legs = int.Parse(key[AccaBoostTierPrefix.Length..], CultureInfo.InvariantCulture);
            return legs < Promotions.AccaBoost.MinLegs || legs > Promotions.AccaBoost.MaxLegs ? "A boost tier is 2 to 20 legs."
                : ParsePercent(value, 100m) is null ? "A boost is 0 to 100 percent." : null;
        }

        if (key == AccaBoostMinLegOdds)
        {
            return ParseOdds(value) is null ? "The minimum leg price is 1.01 to 10." : null;
        }

        if (OddsBoostKey().IsMatch(key))
        {
            return value == "off" || Promotions.OddsBoost.Parse(value) is not null ? null : "A boost is odds;from;until;maxStake, or off.";
        }

        if (FlagKey().IsMatch(key))
        {
            return value is "true" or "false" ? null : "A flag is true or false.";
        }

        return "Unknown setting.";
    }

    [GeneratedRegex("^placement\\.live-delay-seconds\\.[a-z0-9-]{1,40}$")]
    private static partial Regex LiveDelayKey();

    [GeneratedRegex("^limits\\.max-(stake|payout)\\.[A-Z]{3}$")]
    private static partial Regex LimitKey();

    private static decimal? ParsePercent(string? value, decimal max) =>
        decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var percent) && percent >= 0 && percent <= max ? percent : null;

    private static decimal? ParseOdds(string? value) =>
        decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var odds) && odds >= 1.01m && odds <= 10m ? odds : null;

    [GeneratedRegex("^betbuilder\\.competition\\.[a-z0-9-]{1,40}$")]
    private static partial Regex BetBuilderCompetitionKey();

    [GeneratedRegex("^betbuilder\\.correlation\\.[a-z]{1,20}\\.[a-z]{1,20}$")]
    private static partial Regex BetBuilderCorrelationKey();

    [GeneratedRegex("^betbuilder\\.max-payout\\.[A-Z]{3}$")]
    private static partial Regex BetBuilderMaxPayoutKey();

    [GeneratedRegex("^boosts\\.acca\\.legs\\.[0-9]{1,2}$")]
    private static partial Regex AccaTierKey();

    [GeneratedRegex("^boosts\\.odds\\.[a-z0-9-]{1,80}\\.[a-z]{1,20}$")]
    private static partial Regex OddsBoostKey();

    [GeneratedRegex("^flags\\.[a-z0-9-]{1,60}$")]
    private static partial Regex FlagKey();
}
