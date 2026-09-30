using LockLights.Configuration;
using LockLights.Input;
using LockLights.Osd;
using LockLights.Rendering;
using Microsoft.Win32;

namespace LockLights.Tray;

/// <summary>Owns the tray icons, the menu and the popup, and polls the lock keys.</summary>
internal sealed class TrayApplicationContext : ApplicationContext
{
    private const int PollIntervalMilliseconds = 100;

    private readonly UserSettings _settings = UserSettings.Load();
    private readonly OsdWindow _osd = new();
    private readonly TrayMenu _menu;
    private readonly IReadOnlyList<LockKeyIndicator> _indicators;
    private readonly System.Windows.Forms.Timer _pollTimer = new() { Interval = PollIntervalMilliseconds };
    private bool _lightTaskbar = WindowsTheme.IsTaskbarLight();

    public TrayApplicationContext()
    {
        StartupRegistration.RefreshPath();
        _ = _osd.Handle; // create the window up front so the first popup appears instantly

        _menu = new TrayMenu(_settings);
        _menu.SettingsChanged += (_, _) =>
        {
            _settings.Save();
            ApplyVisibility();
        };
        _menu.ExitRequested += (_, _) => ExitThread();

        _indicators = LockKeyDefinition.All.Select(definition => new LockKeyIndicator(definition, _menu)).ToList();

        UpdateIndicators(forceRedraw: true);
        ApplyVisibility();

        _pollTimer.Tick += (_, _) => UpdateIndicators(forceRedraw: false);
        _pollTimer.Start();

        SystemEvents.UserPreferenceChanged += OnSystemChanged;
        SystemEvents.DisplaySettingsChanged += OnSystemChanged;
    }

    protected override void ExitThreadCore()
    {
        _pollTimer.Stop();
        SystemEvents.UserPreferenceChanged -= OnSystemChanged;
        SystemEvents.DisplaySettingsChanged -= OnSystemChanged;

        foreach (var indicator in _indicators)
            indicator.Dispose();
        _osd.Dispose();
        _menu.Dispose();
        _pollTimer.Dispose();

        base.ExitThreadCore();
    }

    private void UpdateIndicators(bool forceRedraw)
    {
        foreach (var indicator in _indicators)
        {
            bool changed = indicator.Update(_lightTaskbar, forceRedraw);
            if (changed && _settings.ShowOsd && _settings.IsVisible(indicator.Definition.Key))
                _osd.ShowState(indicator.Definition, indicator.IsOn == true);
        }
    }

    private void ApplyVisibility()
    {
        foreach (var indicator in _indicators)
            indicator.Visible = _settings.IsVisible(indicator.Definition.Key);
    }

    /// <summary>Theme, DPI or icon size may have changed, so redraw everything.</summary>
    private void OnSystemChanged(object? sender, EventArgs e)
    {
        _lightTaskbar = WindowsTheme.IsTaskbarLight();
        UpdateIndicators(forceRedraw: true);
    }
}
