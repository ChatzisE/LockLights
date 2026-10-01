using Microsoft.Win32;

namespace LockLights.Rendering;

/// <summary>Reads the current Windows light/dark preferences (Settings → Personalization → Colors).</summary>
internal static class WindowsTheme
{
    private const string PersonalizeKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

    /// <summary>"Choose your default Windows mode": the taskbar, and so the tray icons.</summary>
    public static bool IsTaskbarLight() => ReadFlag("SystemUsesLightTheme", defaultValue: false);

    /// <summary>"Choose your default app mode": windows on the desktop, and so the popup.</summary>
    public static bool IsAppsLight() => ReadFlag("AppsUseLightTheme", defaultValue: true);

    private static bool ReadFlag(string valueName, bool defaultValue)
    {
        using var key = Registry.CurrentUser.OpenSubKey(PersonalizeKeyPath);
        return key?.GetValue(valueName) is int value ? value != 0 : defaultValue;
    }
}
