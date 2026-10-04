using System.Text.Json;
using SwiftBets.Contracts.Offer;
using SwiftBets.Contracts.Serialization;

namespace SwiftBets.Contracts.Tests;

public sealed class OfferContractTests
{
    [Fact]
    public void A_line_selection_and_a_market_suffix_resolve_back_to_their_parts()
    {
        var id = SelectionIds.WithLine(SelectionIds.Home, -1.5m);

        SelectionIds.TryParseLine(id, out var side, out var line).ShouldBeTrue();
        (id, side, line).ShouldBe(("home:-1.5", "home", -1.5m));
        MarketIds.TypeOf(MarketIds.For("tennis-c0-m001", MarketType.SetHandicap)).ShouldBe(MarketType.SetHandicap);
    }

    [Fact]
    public void A_snapshot_from_before_sports_reads_as_football_and_a_plain_selection_has_no_line()
    {
        const string json = """{"fixtureId":"f","competition":"Premier League","homeTeam":"A","awayTeam":"B","kickoffAt":"2026-10-02T12:00:00Z","status":"scheduled","offerVersion":1,"markets":[],"changedAt":"2026-10-02T12:00:00Z"}""";

        JsonSerializer.Deserialize<FixtureChangedV1>(json, ContractJson.Options)!.Sport.ShouldBe(Sports.Soccer);
        SelectionIds.TryParseLine(SelectionIds.Home, out _, out _).ShouldBeFalse();
    }
}
