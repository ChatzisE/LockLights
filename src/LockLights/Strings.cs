namespace LockLights;

/// <summary>All user-visible text, kept in one place.</summary>
internal static class Strings
{
    public const string CapsLock = "Caps Lock";
    public const string NumLock = "Num Lock";
    public const string ScrollLock = "Scroll Lock";

    public const string On = "ON";
    public const string Off = "OFF";

    public const string MenuShowKeyFormat = "Show {0}";
    public const string MenuOnScreenPopup = "On-screen popup";
    public const string MenuStartWithWindows = "Start with Windows";
    public const string MenuExit = "Exit";

    public static string State(bool on) => on ? On : Off;

    public static string Tooltip(string keyName, bool on) => $"{keyName}: {State(on)}";
}
