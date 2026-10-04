using SwiftBets.Contracts.Messaging;

namespace SwiftBets.Contracts.Settlement;

/// <summary>One bet's settlement: how its lines fell and what it returns. <see cref="Return"/> includes <see cref="BoostBonus"/>.</summary>
public sealed record BetSettlementV2(
    Guid BetId,
    CouponOutcome Outcome,
    int WinningLines,
    int VoidLines,
    int LosingLines,
    Money.Money Return,
    Money.Money? BoostBonus = null);

/// <summary>
/// A V2 coupon's settlement at <see cref="SettlementVersion"/>. The coupon is Won when it returns more than nothing
/// but not only stakes back, Void when every line is void, otherwise Lost. Payout moves the difference between
/// <see cref="TargetPayout"/> and what it has already paid.
/// </summary>
public sealed record CouponSettledV2(
    Guid CouponId,
    Guid PunterId,
    int SettlementVersion,
    CouponOutcome Outcome,
    Money.Money TotalStake,
    Money.Money TargetPayout,
    IReadOnlyList<BetSettlementV2> Bets,
    DateTimeOffset SettledAt) : IEventContract
{
    public static string EventType => "settlement.coupon-settled";

    public static int EventVersion => 2;
}
