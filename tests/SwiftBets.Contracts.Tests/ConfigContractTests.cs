using System.Text.Json;
using SwiftBets.Contracts.Config;
using SwiftBets.Contracts.Messaging;
using SwiftBets.Contracts.Serialization;

namespace SwiftBets.Contracts.Tests;

public sealed class ConfigContractTests
{
    [Fact]
    public void Entry_round_trips()
    {
        var entry = new ConfigEntryV1(ConfigKeys.PlacementKillSwitch, "on", 3, "ops-1", "feed outage", new DateTimeOffset(2026, 10, 1, 12, 0, 0, TimeSpan.Zero));

        JsonSerializer.Deserialize<ConfigEntryV1>(JsonSerializer.Serialize(entry, ContractJson.Options), ContractJson.Options).ShouldBe(entry);
        Topics.All.ShouldContain(Topics.ConfigEntries);
    }

    [Fact]
    public void Content_change_round_trips()
    {
        var changed = new ContentChangedV1("page", "faq", "ops-1", new DateTimeOffset(2026, 10, 4, 9, 0, 0, TimeSpan.Zero));

        JsonSerializer.Deserialize<ContentChangedV1>(JsonSerializer.Serialize(changed, ContractJson.Options), ContractJson.Options).ShouldBe(changed);
        Topics.All.ShouldContain(Topics.ContentChanged);
    }

    [Fact]
    public void A_content_change_without_its_kind_is_rejected()
    {
        const string json = """{"slug":"faq","changedBy":"ops-1","changedAt":"2026-10-04T09:00:00Z"}""";

        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<ContentChangedV1>(json, ContractJson.Options));
    }

    [Theory]
    [InlineData("placement.kill-switch", "on", true)]
    [InlineData("placement.kill-switch", "yes", false)]
    [InlineData("placement.mode", "preMatchOnly", true)]
    [InlineData("placement.mode", "paused", false)]
    [InlineData("placement.live-delay-seconds.soccer", "8", true)]
    [InlineData("placement.live-delay-seconds.default", "31", false)]
    [InlineData("placement.live-delay-seconds.soccer", "-1", false)]
    [InlineData("limits.max-stake.ZAR", "5000000", true)]
    [InlineData("limits.max-payout.usd", "100", false)]
    [InlineData("limits.max-stake.ZAR", "0", false)]
    [InlineData("flags.cashout", "true", true)]
    [InlineData("flags.cashout", "1", false)]
    [InlineData("something.else", "x", false)]
    public void Values_are_validated_per_key(string key, string value, bool valid) =>
        (ConfigKeys.Validate(key, value) is null).ShouldBe(valid);

    [Fact]
    public void Readers_fall_back_to_defaults()
    {
        ConfigKeys.IsKillSwitchOn(null).ShouldBeFalse();
        ConfigKeys.IsKillSwitchOn("on").ShouldBeTrue();
        ConfigKeys.ParseMode(null).ShouldBe(PlacementMode.Open);
        ConfigKeys.ParseMode("closed").ShouldBe(PlacementMode.Closed);
        ConfigKeys.ParseMinorUnits("abc").ShouldBeNull();
        ConfigKeys.MaxStake("ZAR").ShouldBe("limits.max-stake.ZAR");
        ConfigKeys.LiveDelaySeconds("tennis").ShouldBe("placement.live-delay-seconds.tennis");
        ConfigKeys.Flag("cashout").ShouldBe("flags.cashout");
    }
}
