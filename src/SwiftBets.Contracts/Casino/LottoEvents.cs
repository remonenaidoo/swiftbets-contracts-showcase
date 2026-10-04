using SwiftBets.Contracts.Messaging;

namespace SwiftBets.Contracts.Casino;

/// <summary>Where a lotto draw is in its life: open for bets, closed, resulted, settled, or void.</summary>
public enum LottoDrawStatus
{
    Open = 1,
    Closed = 2,
    Resulted = 3,
    Settled = 4,
    Void = 5,
}

/// <summary>
/// A lotto draw changed status. <c>SeedHash</c> is the SHA-256 of the house seed, published before the draw;
/// <c>Seed</c> is revealed with the result so anyone can recompute the balls. Provider draws carry no seed.
/// </summary>
public sealed record LottoDrawChangedV1(
    Guid DrawId,
    string DrawTypeId,
    LottoDrawStatus Status,
    DateTimeOffset DrawAt,
    DateTimeOffset ClosesAt,
    IReadOnlyList<int>? Balls,
    int? BonusBall,
    string? SeedHash,
    string? Seed,
    DateTimeOffset OccurredAt) : IEventContract
{
    public static string EventType => "casino.lotto-draw-changed";

    public static int EventVersion => 1;
}

/// <summary>One lotto bet settled through the wallet; only its owner hears about it.</summary>
public sealed record LottoBetSettledV1(
    Guid BetId,
    Guid DrawId,
    Guid PunterId,
    bool Won,
    Money.Money Payout,
    DateTimeOffset SettledAt) : IEventContract
{
    public static string EventType => "casino.lotto-bet-settled";

    public static int EventVersion => 1;
}
