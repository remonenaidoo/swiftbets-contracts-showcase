using SwiftBets.Contracts.Messaging;

namespace SwiftBets.Contracts.Config;

/// <summary>
/// Site content changed: <see cref="Kind"/> is "home" (banners or quick links) or "page", with the page slug. It carries
/// no content; realtime tells every open client, which refetches the public content API.
/// </summary>
public sealed record ContentChangedV1(string Kind, string? Slug, string ChangedBy, DateTimeOffset ChangedAt) : IEventContract
{
    public static string EventType => "config.content-changed";

    public static int EventVersion => 1;
}
