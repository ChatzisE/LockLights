using LockLights.Input;
using LockLights.Rendering;

namespace LockLights.Tray;

/// <summary>One tray icon that mirrors the state of one lock key. Left-click toggles the key.</summary>
internal sealed class LockKeyIndicator : IDisposable
{
    private readonly NotifyIcon _notifyIcon = new();
    private Icon? _icon;

    public LockKeyIndicator(LockKeyDefinition definition, ContextMenuStrip menu)
    {
        Definition = definition;
        _notifyIcon.ContextMenuStrip = menu;
        _notifyIcon.MouseClick += (_, e) =>
        {
            if (e.Button == MouseButtons.Left)
                Keyboard.Toggle(Definition.Key);
        };
    }

    public LockKeyDefinition Definition { get; }

    /// <summary>Last known state; null until the first <see cref="Update"/>.</summary>
    public bool? IsOn { get; private set; }

    public bool Visible
    {
        get => _notifyIcon.Visible;
        set => _notifyIcon.Visible = value;
    }

    /// <summary>
    /// Reads the key state and redraws if needed.
    /// Returns true only when the state changed since the previous read.
    /// </summary>
    public bool Update(bool lightTaskbar, bool forceRedraw)
    {
        bool on = Keyboard.IsLocked(Definition.Key);
        bool changed = IsOn.HasValue && IsOn != on;

        if (IsOn == on && !forceRedraw)
            return false;

        IsOn = on;
        Redraw(on, lightTaskbar);
        return changed;
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _icon?.Dispose();
    }

    private void Redraw(bool on, bool lightTaskbar)
    {
        int size = Math.Max(Theme.Tray.MinIconSize, SystemInformation.SmallIconSize.Width);
        var icon = KeyBadgeRenderer.CreateIcon(Definition.Glyph, on, Definition.Accent, Theme.Tray.OffColor(lightTaskbar), size);

        _notifyIcon.Icon = icon;
        _icon?.Dispose();
        _icon = icon;
        _notifyIcon.Text = Strings.Tooltip(Definition.DisplayName, on);
    }
}
