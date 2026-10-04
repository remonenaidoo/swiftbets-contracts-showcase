using System.Text.Json;
using SwiftBets.Contracts.Config;
using SwiftBets.Contracts.Offer;
using SwiftBets.Contracts.Placement;
using SwiftBets.Contracts.Promotions;
using SwiftBets.Contracts.Serialization;

namespace SwiftBets.Contracts.Tests;

public sealed class BetBuilderAndBoostTests
{
    private static readonly BuilderComponentV1 Home = new("f-1x2", SelectionIds.Home, 2.00m);
    private static readonly BuilderComponentV1 Over = new("f-ou25", SelectionIds.Over, 1.80m);

    [Fact]
    public void Correlated_pair_is_priced_above_the_naive_product_less_the_margin()
    {
        var factors = BetBuilderPricing.FactorsFor([Home, Over], new Dictionary<string, decimal>());

        // 2.00 x 1.80 = 3.60, x 1.10 home-over factor = 3.96, less 5% margin = 3.762 -> 3.76.
        BetBuilderPricing.Price([Home, Over], factors, 5m).ShouldBe(3.76m);
        BetBuilderPricing.SelectionId([Over, Home]).ShouldBe("home+over");
    }

    [Fact]
    public void Two_selections_from_one_market_are_refused() =>
        BetBuilderPricing.Problem([Home, new("f-1x2", SelectionIds.Away, 3.4m)]).ShouldNotBeNull();

    [Fact]
    public void Acca_boost_takes_the_highest_tier_reached_and_needs_every_leg_above_the_minimum()
    {
        var boost = new AccaBoost(new Dictionary<int, decimal> { [3] = 5m, [10] = 50m }, 1.5m);

        boost.PercentFor([1.6m, 1.7m, 2m, 2m]).ShouldBe(5m);
        boost.PercentFor([1.6m, 1.4m, 2m, 2m]).ShouldBe(0m);
        AccaBoost.Bonus(1000, 5000, 10m).ShouldBe(400);
    }

    [Fact]
    public void Odds_boost_round_trips_and_a_bad_window_is_refused()
    {
        var boost = new OddsBoost(3.5m, new DateTimeOffset(2026, 10, 4, 10, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 10, 5, 10, 0, 0, TimeSpan.Zero), 5000);

        OddsBoost.Parse(boost.Format()).ShouldBe(boost);
        OddsBoost.Parse("3.5;2026-10-05T10:00:00Z;2026-10-04T10:00:00Z;5000").ShouldBeNull();
    }

    [Theory]
    [InlineData("betbuilder.correlation.home.over", "1.2", true)]
    [InlineData("betbuilder.correlation.over.home", "1.2", false)]
    [InlineData("boosts.acca.legs.3", "5", true)]
    [InlineData("boosts.acca.legs.1", "5", false)]
    [InlineData("boosts.odds.2024-25-c1-m1-1x2.home", "off", true)]
    [InlineData("betbuilder.margin-percent", "45", false)]
    public void Promotion_settings_are_validated(string key, string value, bool valid) =>
        (ConfigKeys.Validate(key, value) is null).ShouldBe(valid);

    [Fact]
    public void A_coupon_from_before_the_builder_still_reads()
    {
        var json = """{"legId":"0199a000-0000-7000-8000-000000000000","fixtureId":"f","marketId":"m","selectionId":"home","odds":2.0,"offerVersion":1,"isBanker":false}""";

        JsonSerializer.Deserialize<CouponLegV2>(json, ContractJson.Options)!.Builder.ShouldBeNull();
    }
}
