using SwiftBets.Contracts.Messaging;

namespace SwiftBets.Contracts.Placement;

/// <summary>
/// A leg of a V2 coupon. A banker is in every line of every bet on the coupon. A bet builder leg has market id
/// <see cref="Offer.BetBuilderPricing.MarketId"/>, its components in <see cref="Builder"/>, and settles as one leg.
/// The display names (<see cref="FixtureName"/> "Home v Away", <see cref="Competition"/>, <see cref="MarketType"/> as the
/// offer's camelCase market type, <see cref="SelectionName"/>) are copied from the offer at placement so bet history can
/// show a leg after its fixture leaves the offer. They are optional: events placed before 2.13.0 carry none.
/// </summary>
public sealed record CouponLegV2(
    Guid LegId,
    string FixtureId,
    string MarketId,
    string SelectionId,
    decimal Odds,
    long OfferVersion,
    bool IsBanker,
    Offer.BetBuilderLegV1? Builder = null,
    string? FixtureName = null,
    string? Competition = null,
    string? MarketType = null,
    string? SelectionName = null);

/// <summary>
/// One bet on a coupon: every combination of each size in <see cref="Folds"/> taken from the non-banker legs, with the
/// bankers added to each. A Trixie on three selections is folds [2, 3]; singles are [1]; an accumulator of n is [n].
/// <see cref="Stake"/> is <see cref="UnitStake"/> times <see cref="Lines"/>. <see cref="AccaBoostPercent"/> is the share of
/// the winnings settlement adds when the bet wins, fixed at placement (see <see cref="Promotions.AccaBoost"/>).
/// </summary>
public sealed record CouponBetV2(
    Guid BetId,
    string Name,
    IReadOnlyList<int> Folds,
    int Lines,
    Money.Money UnitStake,
    Money.Money Stake,
    Money.Money PotentialPayout,
    decimal AccaBoostPercent = 0m);

/// <summary>
/// A placed coupon with one or more bets over its legs. <see cref="PromoStake"/> is the part of <see cref="TotalStake"/>
/// the wallet took from bonus money or a free bet rather than the customer's cash (2.14.0); null on older events, and
/// null or zero means the whole stake was cash.
/// </summary>
public sealed record CouponPlacedV2(
    Guid CouponId,
    Guid PunterId,
    Money.Money TotalStake,
    Money.Money PotentialPayout,
    IReadOnlyList<CouponLegV2> Legs,
    IReadOnlyList<CouponBetV2> Bets,
    DateTimeOffset PlacedAt,
    Money.Money? PromoStake = null) : IEventContract
{
    public static string EventType => "placement.coupon-placed";

    public static int EventVersion => 2;
}
