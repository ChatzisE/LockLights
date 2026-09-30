using LockLights.Input;
using Microsoft.Win32;

namespace LockLights.Configuration;

/// <summary>Per-user preferences, stored under HKCU\Software\LockLights.</summary>
internal sealed class UserSettings
{
    private const string ShowOsdValueName = "ShowOsd";

    private readonly Dictionary<Keys, bool> _visibleKeys = [];

    public bool ShowOsd { get; set; } = true;

    public int VisibleKeyCount => _visibleKeys.Values.Count(visible => visible);

    public bool IsVisible(Keys key) => _visibleKeys.TryGetValue(key, out bool visible) && visible;

    public void SetVisible(Keys key, bool visible) => _visibleKeys[key] = visible;

    public static UserSettings Load()
    {
        var settings = new UserSettings();
        using var key = Registry.CurrentUser.OpenSubKey(AppInfo.RegistryPath);

        settings.ShowOsd = ReadBool(key, ShowOsdValueName, defaultValue: true);
        foreach (var definition in LockKeyDefinition.All)
            settings.SetVisible(definition.Key, ReadBool(key, VisibleValueName(definition.Key), definition.VisibleByDefault));

        // With no icon at all there'd be no way to reach the menu again.
        if (settings.VisibleKeyCount == 0)
            settings.SetVisible(LockKeyDefinition.All[0].Key, true);

        return settings;
    }

    public void Save()
    {
        using var key = Registry.CurrentUser.CreateSubKey(AppInfo.RegistryPath);

        WriteBool(key, ShowOsdValueName, ShowOsd);
        foreach (var (lockKey, visible) in _visibleKeys)
            WriteBool(key, VisibleValueName(lockKey), visible);
    }

    private static string VisibleValueName(Keys key) => $"Show{key}";

    private static bool ReadBool(RegistryKey? key, string name, bool defaultValue) =>
        key?.GetValue(name) is int value ? value != 0 : defaultValue;

    private static void WriteBool(RegistryKey key, string name, bool value) =>
        key.SetValue(name, value ? 1 : 0, RegistryValueKind.DWord);
}
