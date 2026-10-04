using System.Text.Json;
using SwiftBets.Contracts.Casino;
using SwiftBets.Contracts.Messaging;
using SwiftBets.Contracts.Serialization;

namespace SwiftBets.Contracts.Tests;

public sealed class CasinoContractTests
{
    [Fact]
    public void A_casino_transaction_round_trips_with_its_kind_by_name()
    {
        var tx = new CasinoTransactionV1(Guid.NewGuid(), "sim-seamless", "p-1", "r-1", Guid.NewGuid(), "sun-temple", CasinoTransactionKind.FreeSpinBet,
            new Money.Money(0, "ZAR"), new DateTimeOffset(2026, 10, 3, 12, 0, 0, TimeSpan.Zero));

        var json = JsonSerializer.Serialize(tx, ContractJson.Options);

        json.ShouldContain("\"kind\":\"freeSpinBet\"");
        JsonSerializer.Deserialize<CasinoTransactionV1>(json, ContractJson.Options).ShouldBe(tx);
        Topics.All.ShouldContain(Topics.CasinoTransaction);
    }

    [Fact]
    public void A_reconciliation_without_its_status_is_rejected()
    {
        const string json = """{"providerId":"sim","businessDate":"2026-10-02","ourNet":{"minorUnits":0,"currency":"ZAR"},"providerNet":{"minorUnits":0,"currency":"ZAR"},"drift":{"minorUnits":0,"currency":"ZAR"},"missingOnOurSide":0,"missingOnProviderSide":0,"reconciledAt":"2026-10-03T00:00:00Z"}""";

        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<ProviderReconciliationV1>(json, ContractJson.Options));
    }

    [Fact]
    public void A_lotto_draw_round_trips_with_its_balls_and_revealed_seed()
    {
        var draw = new LottoDrawChangedV1(Guid.NewGuid(), "swift-rapid", LottoDrawStatus.Resulted, new DateTimeOffset(2026, 10, 4, 12, 5, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 10, 4, 12, 4, 30, TimeSpan.Zero), [3, 9, 14, 22, 30, 35], 7, "ab12", "seed", new DateTimeOffset(2026, 10, 4, 12, 5, 1, TimeSpan.Zero));

        var json = JsonSerializer.Serialize(draw, ContractJson.Options);

        json.ShouldContain("\"status\":\"resulted\"");
        var back = JsonSerializer.Deserialize<LottoDrawChangedV1>(json, ContractJson.Options)!;
        (back with { Balls = draw.Balls }).ShouldBe(draw);
        back.Balls.ShouldBe(draw.Balls);
        Topics.All.ShouldContain(Topics.LottoDrawChanged);
    }

    [Fact]
    public void A_lotto_bet_settlement_without_its_punter_is_rejected()
    {
        const string json = """{"betId":"9f1c3c84-1d5e-4a8e-9a39-2f3d1c0e8a11","drawId":"9f1c3c84-1d5e-4a8e-9a39-2f3d1c0e8a12","won":true,"payout":{"minorUnits":100,"currency":"ZAR"},"settledAt":"2026-10-04T12:05:00Z"}""";

        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<LottoBetSettledV1>(json, ContractJson.Options));
    }
}
