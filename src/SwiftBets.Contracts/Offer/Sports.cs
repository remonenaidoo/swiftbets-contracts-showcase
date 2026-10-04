namespace SwiftBets.Contracts.Offer;

/// <summary>Sport ids as the catalogue and fixtures carry them.</summary>
public static class Sports
{
    public const string Soccer = "soccer";
    public const string Tennis = "tennis";
    public const string RugbyUnion = "rugby-union";
    public const string Cricket = "cricket";
    public const string Basketball = "basketball";

    public static IReadOnlyList<string> All { get; } = [Soccer, Tennis, RugbyUnion, Cricket, Basketball];

    public static string Name(string sportId) => sportId switch
    {
        Soccer => "Football",
        Tennis => "Tennis",
        RugbyUnion => "Rugby union",
        Cricket => "Cricket",
        Basketball => "Basketball",
        _ => sportId,
    };
}
