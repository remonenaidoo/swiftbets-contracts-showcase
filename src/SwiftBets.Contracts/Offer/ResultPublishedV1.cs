using SwiftBets.Contracts.Messaging;

namespace SwiftBets.Contracts.Offer;

/// <summary>A result; the score is goals, points, runs or (tennis) sets by sport, with <see cref="Detail"/> for the rest.</summary>
public sealed record ResultPublishedV1(
    string FixtureId,
    int ResultVersion,
    ResultStatus Status,
    int HomeGoals,
    int AwayGoals,
    DateTimeOffset PublishedAt,
    ResultDetailV1? Detail = null) : IEventContract
{
    public static string EventType => "offer.result-published";

    public static int EventVersion => 1;
}
