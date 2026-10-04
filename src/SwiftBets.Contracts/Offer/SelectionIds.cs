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
