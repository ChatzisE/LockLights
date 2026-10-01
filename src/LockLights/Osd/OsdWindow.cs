using LockLights.Input;
using LockLights.Interop;
using LockLights.Rendering;

namespace LockLights.Osd;

/// <summary>
/// Small popup shown near the bottom of the active screen when a lock key changes.
/// It never takes focus and clicks pass through it.
/// </summary>
internal sealed class OsdWindow : Form
{
    private const TextFormatFlags TextFlags =
        TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding;

    private readonly System.Windows.Forms.Timer _holdTimer = new() { Interval = Theme.Osd.HoldMilliseconds };
    private readonly System.Windows.Forms.Timer _fadeTimer = new() { Interval = Theme.Osd.FadeIntervalMilliseconds };

    private LockKeyDefinition? _key;
    private bool _isOn;
    private Theme.OsdPalette _palette = Theme.Osd.Dark;
    private float _scale;
    private Font? _nameFont;
    private Font? _stateFont;

    public OsdWindow()
    {
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        AutoScaleMode = AutoScaleMode.None; // we scale manually per monitor
        BackColor = _palette.Background;
        DoubleBuffered = true;

        _holdTimer.Tick += (_, _) =>
        {
            _holdTimer.Stop();
            _fadeTimer.Start();
        };
        _fadeTimer.Tick += (_, _) => FadeStep();
    }

    protected override bool ShowWithoutActivation => true;

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= NativeMethods.WS_EX_TOOLWINDOW | NativeMethods.WS_EX_NOACTIVATE |
                          NativeMethods.WS_EX_TOPMOST | NativeMethods.WS_EX_TRANSPARENT;
            return cp;
        }
    }

    public void ShowState(LockKeyDefinition key, bool isOn)
    {
        _key = key;
        _isOn = isOn;
        _palette = Theme.Osd.Palette(WindowsTheme.IsAppsLight()); // read on every show to follow the current OS mode
        BackColor = _palette.Background;

        Rectangle workArea = Screen.FromPoint(Cursor.Position).WorkingArea;
        SetScale(ScreenDpi.ScaleAt(new Point(workArea.Left + workArea.Width / 2, workArea.Top + workArea.Height / 2)));

        int width = Scaled(Theme.Osd.Width);
        int height = Scaled(Theme.Osd.Height);
        int x = workArea.Left + (workArea.Width - width) / 2;
        int y = workArea.Bottom - height - Scaled(Theme.Osd.BottomMargin);

        _fadeTimer.Stop();
        _holdTimer.Stop();
        Opacity = Theme.Osd.Opacity;
        SetBounds(x, y, width, height);
        ApplyRoundedRegion(width, height);

        if (!Visible)
            Show();
        Invalidate();
        _holdTimer.Start();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (_key is null || _nameFont is null || _stateFont is null)
            return;

        var g = e.Graphics;
        int height = ClientSize.Height;

        int badgeSize = Scaled(Theme.Osd.BadgeSize);
        var badgeBounds = new RectangleF(Scaled(Theme.Osd.PaddingLeft), (height - badgeSize) / 2f, badgeSize, badgeSize);
        KeyBadgeRenderer.Draw(g, badgeBounds, _key.Glyph, _isOn, _key.Accent, _palette.OffText);

        int textLeft = (int)badgeBounds.Right + Scaled(Theme.Osd.BadgeGap);
        int textRight = ClientSize.Width - Scaled(Theme.Osd.PaddingRight);
        var textBounds = new Rectangle(textLeft, 0, textRight - textLeft, height);

        TextRenderer.DrawText(g, _key.DisplayName, _nameFont, textBounds,
            _palette.Text, _palette.Background, TextFlags | TextFormatFlags.Left);
        TextRenderer.DrawText(g, Strings.State(_isOn), _stateFont, textBounds,
            _isOn ? _palette.Text : _palette.OffText, _palette.Background, TextFlags | TextFormatFlags.Right);

        DrawBorder(g);
    }

    /// <summary>Thin outline so the popup stays visible on a background of the same color.</summary>
    private void DrawBorder(Graphics g)
    {
        float width = Math.Max(1f, Theme.Osd.BorderWidth * _scale);
        var bounds = RectangleF.Inflate(new RectangleF(0, 0, ClientSize.Width, ClientSize.Height), -width / 2, -width / 2);
        using var path = Shapes.RoundedRect(bounds, Scaled(Theme.Osd.CornerRadius) - width / 2);
        using var pen = new Pen(_palette.Border, width);
        g.DrawPath(pen, path);
    }

    protected override void WndProc(ref Message m)
    {
        // We size ourselves for the target monitor; ignore the rect Windows suggests on DPI change.
        if (m.Msg == NativeMethods.WM_DPICHANGED)
        {
            m.Result = IntPtr.Zero;
            return;
        }
        base.WndProc(ref m);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _holdTimer.Dispose();
            _fadeTimer.Dispose();
            _nameFont?.Dispose();
            _stateFont?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void FadeStep()
    {
        if (Opacity <= Theme.Osd.FadeStep)
        {
            _fadeTimer.Stop();
            Hide();
        }
        else
        {
            Opacity -= Theme.Osd.FadeStep;
        }
    }

    /// <summary>Recreates the fonts only when the monitor scale actually changes.</summary>
    private void SetScale(float scale)
    {
        if (scale == _scale && _nameFont is not null)
            return;

        _scale = scale;
        _nameFont?.Dispose();
        _stateFont?.Dispose();
        _nameFont = Theme.CreateFont(Theme.Osd.NameFontSize * scale, Theme.Osd.NameFontStyle);
        _stateFont = Theme.CreateFont(Theme.Osd.StateFontSize * scale, Theme.Osd.StateFontStyle);
    }

    private void ApplyRoundedRegion(int width, int height)
    {
        using var path = Shapes.RoundedRect(new RectangleF(0, 0, width, height), Scaled(Theme.Osd.CornerRadius));
        var previous = Region;
        Region = new Region(path);
        previous?.Dispose();
    }

    private int Scaled(int value) => (int)Math.Round(value * _scale);
}
