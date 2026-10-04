namespace SwiftBets.Contracts.Offer;

/// <summary>A market and its prices; <c>Line</c> is the handicap or total for line markets, shown beside the market name.</summary>
public sealed record MarketV1(string MarketId, MarketType Type, MarketStatus Status, IReadOnlyList<SelectionV1> Selections, decimal? Line = null);
