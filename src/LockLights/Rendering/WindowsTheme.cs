using Microsoft.Win32;

namespace LockLights.Rendering;

/// <summary>Reads the current Windows light/dark preferences.</summary>
internal static class WindowsTheme
{
    private const string PersonalizeKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

    /// <summary>True when the taskbar (and so the tray) uses the light theme.</summary>
    public static bool IsTaskbarLight()
    {
        using var key = Registry.CurrentUser.OpenSubKey(PersonalizeKeyPath);
        return key?.GetValue("SystemUsesLightTheme") is int value && value != 0;
    }
}
