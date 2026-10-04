using System.Globalization;

namespace SwiftBets.Contracts.Offer;

/// <summary>One selection inside a bet builder, at the price it had on its own when the builder was priced.</summary>
public sealed record BuilderComponentV1(string MarketId, string SelectionId, decimal Odds);

/// <summary>
/// The pricing inputs a bet builder leg was accepted on: its components, the correlation factor of every pair (keyed by
/// <see cref="BetBuilderPricing.PairKey"/>) and the margin. Settlement reprices from these when a component is void.
/// </summary>
public sealed record BetBuilderLegV1(IReadOnlyList<BuilderComponentV1> Components, IReadOnlyDictionary<string, decimal> PairFactors, decimal MarginPercent);

/// <summary>
/// The first bet builder model, shared so the offer quote, placement and settlement price alike. The naive price is the
/// product of the component odds; each pair of components is then multiplied by its correlation factor (above 1 when
/// the two tend to happen together, below 1 when they pull apart) and the margin is taken off. The result rounds down
/// to two decimals and is never below 1.01. Two components from the same market contradict each other and are refused.
/// </summary>
public static class BetBuilderPricing
{
    /// <summary>The market id a bet builder leg carries on a coupon.</summary>
    public const string MarketId = "bet-builder";

    public const int MinComponents = 2;
    public const int MaxComponents = 6;
    public const decimal DefaultMarginPercent = 5m;
    public const decimal MinFactor = 0.1m;
    public const decimal MaxFactor = 3m;

    /// <summary>Built-in factors for the markets on offer, used for any pair the console has not set.</summary>
    public static IReadOnlyDictionary<string, decimal> DefaultFactors { get; } = new Dictionary<string, decimal>(StringComparer.Ordinal)
    {
        [PairKey(SelectionIds.Home, SelectionIds.Over)] = 1.10m,
        [PairKey(SelectionIds.Away, SelectionIds.Over)] = 1.08m,
        [PairKey(SelectionIds.Home, SelectionIds.Under)] = 0.95m,
        [PairKey(SelectionIds.Away, SelectionIds.Under)] = 0.95m,
        [PairKey(SelectionIds.Draw, SelectionIds.Under)] = 1.20m,
        [PairKey(SelectionIds.Draw, SelectionIds.Over)] = 0.80m,
    };

    /// <summary>A pair's key: the two selection ids in ordinal order, joined by a dot.</summary>
    public static string PairKey(string a, string b) => string.CompareOrdinal(a, b) <= 0 ? $"{a}.{b}" : $"{b}.{a}";

    /// <summary>Why these components cannot be combined, or null when they can.</summary>
    public static string? Problem(IReadOnlyList<BuilderComponentV1> components)
    {
        ArgumentNullException.ThrowIfNull(components);
        if (components.Count < MinComponents || components.Count > MaxComponents)
        {
            return $"A bet builder has {MinComponents} to {MaxComponents} selections.";
        }

        return components.Select(c => c.MarketId).Distinct(StringComparer.Ordinal).Count() != components.Count
            ? "Those selections cannot all win together. Pick one per market."
            : null;
    }

    /// <summary>The factor of every pair, from the configured table, else the built-in one, else 1.</summary>
    public static IReadOnlyDictionary<string, decimal> FactorsFor(IReadOnlyList<BuilderComponentV1> components, IReadOnlyDictionary<string, decimal> configured)
    {
        ArgumentNullException.ThrowIfNull(components);
        ArgumentNullException.ThrowIfNull(configured);
        var factors = new Dictionary<string, decimal>(StringComparer.Ordinal);
        for (var i = 0; i < components.Count; i++)
        {
            for (var j = i + 1; j < components.Count; j++)
            {
                var key = PairKey(components[i].SelectionId, components[j].SelectionId);
                factors[key] = configured.TryGetValue(key, out var set) ? set : DefaultFactors.GetValueOrDefault(key, 1m);
            }
        }

        return factors;
    }

    /// <summary>The combined price of these components under these factors and margin.</summary>
    public static decimal Price(IReadOnlyList<BuilderComponentV1> components, IReadOnlyDictionary<string, decimal> factors, decimal marginPercent)
    {
        ArgumentNullException.ThrowIfNull(components);
        ArgumentNullException.ThrowIfNull(factors);
        var price = components.Aggregate(1m, (total, c) => total * c.Odds);
        for (var i = 0; i < components.Count; i++)
        {
            for (var j = i + 1; j < components.Count; j++)
            {
                price *= factors.GetValueOrDefault(PairKey(components[i].SelectionId, components[j].SelectionId), 1m);
            }
        }

        price *= 1m - (marginPercent / 100m);
        return Math.Max(1.01m, decimal.Floor(price * 100m) / 100m);
    }

    /// <summary>The builder's selection id on a coupon: its selection ids in market order, joined by a plus.</summary>
    public static string SelectionId(IReadOnlyList<BuilderComponentV1> components)
    {
        ArgumentNullException.ThrowIfNull(components);
        return string.Join('+', components.OrderBy(c => c.MarketId, StringComparer.Ordinal).Select(c => c.SelectionId));
    }

    /// <summary>The price a leg carries once some components are void: the rest repriced, or null when fewer than two remain and the leg is void.</summary>
    public static decimal? Reprice(BetBuilderLegV1 leg, IReadOnlySet<string> voidSelectionIds)
    {
        ArgumentNullException.ThrowIfNull(leg);
        ArgumentNullException.ThrowIfNull(voidSelectionIds);
        var remaining = leg.Components.Where(c => !voidSelectionIds.Contains(c.SelectionId)).ToList();
        return remaining.Count < MinComponents ? null : Price(remaining, leg.PairFactors, leg.MarginPercent);
    }

    internal static decimal? ParseFactor(string? value) =>
        decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var factor) && factor >= MinFactor && factor <= MaxFactor ? factor : null;
}
