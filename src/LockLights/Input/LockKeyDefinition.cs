using LockLights.Rendering;

namespace LockLights.Input;

/// <summary>Describes one lock key the app can show.</summary>
internal sealed record LockKeyDefinition(Keys Key, string DisplayName, string Glyph, Color Accent, bool VisibleByDefault)
{
    public static IReadOnlyList<LockKeyDefinition> All { get; } =
    [
        new(Keys.CapsLock, Strings.CapsLock, "A", Theme.Accent.CapsLock, VisibleByDefault: true),
        new(Keys.NumLock, Strings.NumLock, "1", Theme.Accent.NumLock, VisibleByDefault: true),
        new(Keys.Scroll, Strings.ScrollLock, "S", Theme.Accent.ScrollLock, VisibleByDefault: false),
    ];
}
