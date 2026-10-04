namespace SwiftBets.Contracts.Offer;

/// <summary>What a market prices; <see cref="MarketIds"/> maps each to the market id suffix settlement reads it from.</summary>
public enum MarketType
{
    MatchResult,
    TotalGoalsOverUnder25,

    /// <summary>Two-way winner with no draw: tennis and cricket match winner, basketball moneyline. A tie voids it.</summary>
    MatchWinner,

    /// <summary>Tennis sets handicap; the line sits in the selection id (<c>home:-1.5</c>).</summary>
    SetHandicap,

    /// <summary>Tennis total games over or under the line in the selection id (<c>over:22.5</c>).</summary>
    TotalGames,

    /// <summary>Points handicap (rugby, basketball spread); a whole-number line can push, which voids the bet.</summary>
    Handicap,

    /// <summary>Total points over or under the line (rugby, basketball); a whole-number line can push.</summary>
    TotalPoints,

    /// <summary>Cricket top batter: one selection per player, won when the result names the player.</summary>
    TopBatter,

    /// <summary>Outright: one selection per team, settled when the competition ends.</summary>
    CompetitionWinner,

    /// <summary>Both teams to score: selections <c>yes</c> and <c>no</c>.</summary>
    BothTeamsToScore,

    /// <summary>Double chance: <c>homeDraw</c>, <c>homeAway</c>, <c>drawAway</c>.</summary>
    DoubleChance,

    /// <summary>Draw no bet: <c>home</c> or <c>away</c>; a draw voids it.</summary>
    DrawNoBet,

    /// <summary>Total goals over or under 1.5; the line sits in the selection id (<c>over:1.5</c>).</summary>
    TotalGoalsOverUnder15,

    /// <summary>Total goals over or under 3.5; the line sits in the selection id (<c>over:3.5</c>).</summary>
    TotalGoalsOverUnder35,

    /// <summary>Correct score: every score up to <see cref="SelectionIds.CorrectScoreMax"/> goals a side (<c>score:2-1</c>), and <c>other</c> for any score with a side above it.</summary>
    CorrectScore,
}
