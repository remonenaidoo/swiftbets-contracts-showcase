namespace SwiftBets.Contracts.Offer;

/// <summary>
/// What a score alone cannot settle: tennis games (the score fields carry sets) and the winning selection ids of
/// player and outright markets, such as the top batter or the competition winner.
/// </summary>
public sealed record ResultDetailV1(int? HomeGames = null, int? AwayGames = null, IReadOnlyList<string>? Winners = null);
