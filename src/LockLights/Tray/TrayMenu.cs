using LockLights.Configuration;
using LockLights.Input;

namespace LockLights.Tray;

/// <summary>The right-click menu shared by all tray icons.</summary>
internal sealed class TrayMenu : ContextMenuStrip
{
    public TrayMenu(UserSettings settings)
    {
        Items.Add(new ToolStripMenuItem(AppInfo.Name) { Enabled = false });
        Items.Add(new ToolStripSeparator());

        foreach (var definition in LockKeyDefinition.All)
        {
            AddToggle(
                string.Format(Strings.MenuShowKeyFormat, definition.DisplayName),
                () => settings.IsVisible(definition.Key),
                visible =>
                {
                    // Keep at least one icon, otherwise the menu can't be reached again.
                    if (!visible && settings.VisibleKeyCount == 1)
                        return false;
                    settings.SetVisible(definition.Key, visible);
                    return true;
                });
        }

        Items.Add(new ToolStripSeparator());

        AddToggle(Strings.MenuOnScreenPopup, () => settings.ShowOsd, value =>
        {
            settings.ShowOsd = value;
            return true;
        });
        AddToggle(Strings.MenuStartWithWindows, () => StartupRegistration.IsEnabled, value =>
        {
            StartupRegistration.IsEnabled = value;
            return true;
        });

        Items.Add(new ToolStripSeparator());
        Items.Add(Strings.MenuExit, null, (_, _) => ExitRequested?.Invoke(this, EventArgs.Empty));
    }

    /// <summary>Raised after the user changes any setting from the menu.</summary>
    public event EventHandler? SettingsChanged;

    public event EventHandler? ExitRequested;

    /// <param name="trySet">Applies the new value; returns false to reject it.</param>
    private void AddToggle(string text, Func<bool> get, Func<bool, bool> trySet)
    {
        var item = new ToolStripMenuItem(text) { Checked = get() };
        item.Click += (_, _) =>
        {
            if (!trySet(!get()))
                return;
            item.Checked = get();
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        };
        Items.Add(item);
    }
}
