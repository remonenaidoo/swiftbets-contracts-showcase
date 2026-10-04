using System.Globalization;

namespace SwiftBets.Contracts.Offer;

/// <summary>Selection ids are part of the offer contract: settlement resolves outcomes from them.</summary>
public static class SelectionIds
{
    public const string Home = "home";
    public const string Draw = "draw";
    public const string Away = "away";
    public const string Over = "over";
    public const string Under = "under";
    public const string Yes = "yes";
    public const string No = "no";
    public const string HomeDraw = "homeDraw";
    public const string HomeAway = "homeAway";
    public const string DrawAway = "drawAway";

    /// <summary>Correct score lists every score up to this many goals a side; settlement reads <see cref="OtherScore"/> from it.</summary>
    public const int CorrectScoreMax = 3;

    /// <summary>Correct score's catch-all: any score without its own selection.</summary>
    public const string OtherScore = "other";

    /// <summary>A correct-score selection: <c>score:2-1</c>, home goals first.</summary>
    public static string Score(int home, int away) => $"score:{home.ToString(CultureInfo.InvariantCulture)}-{away.ToString(CultureInfo.InvariantCulture)}";

    /// <summary>A line selection: <c>home:-1.5</c>, <c>away:+4.5</c>, <c>over:215.5</c>.</summary>
    public static string WithLine(string side, decimal line) =>
        $"{side}:{(side is Over or Under ? line.ToString("0.0", CultureInfo.InvariantCulture) : line.ToString("+0.0;-0.0;0.0", CultureInfo.InvariantCulture))}";

    public static bool TryParseLine(string selectionId, out string side, out decimal line)
    {
        var colon = selectionId.IndexOf(':', StringComparison.Ordinal);
        side = colon < 0 ? selectionId : selectionId[..colon];
        line = 0m;
        return colon > 0 && decimal.TryParse(selectionId[(colon + 1)..], NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out line);
    }
}
