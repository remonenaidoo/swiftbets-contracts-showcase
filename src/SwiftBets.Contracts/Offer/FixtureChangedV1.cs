using SwiftBets.Contracts.Messaging;

namespace SwiftBets.Contracts.Offer;

/// <summary>Full snapshot of a fixture's offer; <see cref="OfferVersion"/> increases on every price or status change.</summary>
public sealed record FixtureChangedV1(
    string FixtureId,
    string Competition,
    string HomeTeam,
    string AwayTeam,
    DateTimeOffset KickoffAt,
    FixtureStatus Status,
    long OfferVersion,
    IReadOnlyList<MarketV1> Markets,
    DateTimeOffset ChangedAt,
    string Sport = Sports.Soccer) : IEventContract
{
    public static string EventType => "offer.fixture-changed";

    public static int EventVersion => 1;
}
