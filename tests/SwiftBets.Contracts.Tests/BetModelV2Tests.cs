using System.Text.Json;
using SwiftBets.Contracts.Messaging;
using SwiftBets.Contracts.Placement;
using SwiftBets.Contracts.Serialization;
using SwiftBets.Contracts.Settlement;

namespace SwiftBets.Contracts.Tests;

public sealed class BetModelV2Tests
{
    private static readonly DateTimeOffset At = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData("trixie", 4)]
    [InlineData("patent", 7)]
    [InlineData("yankee", 11)]
    [InlineData("lucky15", 15)]
    [InlineData("canadian", 26)]
    [InlineData("lucky31", 31)]
    [InlineData("heinz", 57)]
    [InlineData("lucky63", 63)]
    [InlineData("superHeinz", 120)]
    [InlineData("goliath", 247)]
    public void Named_bets_have_their_textbook_line_counts(string name, int lines)
    {
        var (selections, folds) = SystemBets.Named[name];

        SystemBets.Lines(selections, folds).ShouldBe(lines);
        SystemBets.Combinations(selections, folds).Count().ShouldBe(lines);
    }

    [Fact]
    public void A_trixie_is_three_doubles_and_a_treble()
    {
        SystemBets.Combinations(3, [3, 2]).ShouldBe([[0, 1], [0, 2], [1, 2], [0, 1, 2]]);
    }

    [Fact]
    public void Folds_are_validated_against_the_selections()
    {
        SystemBets.Validate(3, [2, 3]).ShouldBeNull();
        SystemBets.Validate(3, []).ShouldNotBeNull();
        SystemBets.Validate(3, [2, 2]).ShouldNotBeNull();
        SystemBets.Validate(3, [4]).ShouldNotBeNull();
        SystemBets.Lines(3, [4]).ShouldBe(0);
        SystemBets.Combinations(3, [0, 4]).ShouldBeEmpty();
    }

    [Fact]
    public void Coupon_events_round_trip_with_bankers_and_bets()
    {
        var zar = (long minor) => new Money.Money(minor, "ZAR");
        var placed = new CouponPlacedV2(Guid.NewGuid(), Guid.NewGuid(), zar(400), zar(9_000),
            [new CouponLegV2(Guid.NewGuid(), "fx-1", "fx-1-1x2", "home", 2.0m, 3, IsBanker: true), new CouponLegV2(Guid.NewGuid(), "fx-2", "fx-2-1x2", "draw", 3.0m, 4, IsBanker: false)],
            [new CouponBetV2(Guid.NewGuid(), "trixie", [2, 3], 4, zar(100), zar(400), zar(9_000))], At);
        var settled = new CouponSettledV2(placed.CouponId, placed.PunterId, 1, CouponOutcome.Won, zar(400), zar(1_200),
            [new BetSettlementV2(placed.Bets[0].BetId, CouponOutcome.Won, 1, 0, 3, zar(1_200))], At);

        var placedBack = JsonSerializer.Deserialize<CouponPlacedV2>(JsonSerializer.Serialize(placed, ContractJson.Options), ContractJson.Options)!;
        placedBack.Legs.ShouldBe(placed.Legs);
        placedBack.Bets[0].Folds.ShouldBe([2, 3]);
        JsonSerializer.Deserialize<CouponSettledV2>(JsonSerializer.Serialize(settled, ContractJson.Options), ContractJson.Options)!.Bets.ShouldBe(settled.Bets);
        CouponPlacedV2.EventType.ShouldBe("placement.coupon-placed");
        CouponPlacedV2.EventVersion.ShouldBe(2);
        Topics.All.ShouldContain(Topics.CouponPlacedV2);
        Topics.All.ShouldContain(Topics.CouponSettledV2);
    }

    [Fact]
    public void Coupon_leg_names_round_trip()
    {
        var leg = new CouponLegV2(Guid.NewGuid(), "fx-1", "fx-1-1x2", "home", 2.0m, 3, false,
            FixtureName: "Chiefs v Pirates", Competition: "Premiership", MarketType: "matchResult", SelectionName: "Chiefs");

        var back = JsonSerializer.Deserialize<CouponLegV2>(JsonSerializer.Serialize(leg, ContractJson.Options), ContractJson.Options)!;

        back.ShouldBe(leg);
    }

    [Fact]
    public void A_leg_placed_before_names_existed_still_reads_with_null_names()
    {
        const string old = """{"legId":"0199a000-0000-7000-8000-000000000001","fixtureId":"fx-1","marketId":"fx-1-1x2","selectionId":"home","odds":2.0,"offerVersion":3,"isBanker":false}""";

        var leg = JsonSerializer.Deserialize<CouponLegV2>(old, ContractJson.Options)!;

        leg.SelectionId.ShouldBe("home");
        leg.FixtureName.ShouldBeNull();
        leg.SelectionName.ShouldBeNull();
    }

    [Fact]
    public void Envelopes_always_carry_a_context_and_older_ones_read_as_platform()
    {
        var payload = new CouponRejectedV1(Guid.NewGuid(), Guid.NewGuid(), "price_changed", At);
        var platform = EventEnvelope<CouponRejectedV1>.Create(payload, At, "corr-1");
        var web = EventEnvelope<CouponRejectedV1>.Create(payload, At, "corr-1", null, new EventContext("swiftbets", "ZA", "web"));

        platform.Context.ShouldBe(EventContext.Platform);
        web.Context!.Channel.ShouldBe("web");
        var json = JsonSerializer.Serialize(web, ContractJson.Options);
        json.ShouldContain("\"context\":{\"brand\":\"swiftbets\",\"country\":\"ZA\",\"channel\":\"web\"}");
        json.ShouldNotContain("contextOrPlatform");

        var older = JsonSerializer.Deserialize<EventEnvelope<CouponRejectedV1>>(json.Replace(",\"context\":{\"brand\":\"swiftbets\",\"country\":\"ZA\",\"channel\":\"web\"}", string.Empty, StringComparison.Ordinal), ContractJson.Options)!;
        older.Context.ShouldBeNull();
        older.ContextOrPlatform.ShouldBe(EventContext.Platform);
    }
}
