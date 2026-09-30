using Microsoft.Win32;

namespace LockLights.Configuration;

/// <summary>"Start with Windows" via the per-user Run key. No admin rights needed.</summary>
internal static class StartupRegistration
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

    private static string Command => $"\"{Environment.ProcessPath}\"";

    public static bool IsEnabled
    {
        get
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath);
            return key?.GetValue(AppInfo.Name) is string;
        }
        set
        {
            using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath);
            if (value)
                key.SetValue(AppInfo.Name, Command);
            else
                key.DeleteValue(AppInfo.Name, throwOnMissingValue: false);
        }
    }

    /// <summary>Keeps the entry pointing at the current exe if the file was moved.</summary>
    public static void RefreshPath()
    {
        if (IsEnabled)
            IsEnabled = true;
    }
}
