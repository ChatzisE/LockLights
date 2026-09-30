namespace LockLights.Rendering;

/// <summary>
/// Every font, color, size and timing the UI uses.
/// Sizes are in pixels at 96 DPI; callers scale them for the current display.
/// </summary>
internal static class Theme
{
    private const string PreferredFontFamily = "Segoe UI";

    public static FontFamily FontFamily { get; } = LoadFontFamily(PreferredFontFamily);

    public static Font CreateFont(float pixelSize, FontStyle style = FontStyle.Regular) =>
        new(FontFamily, pixelSize, style, GraphicsUnit.Pixel);

    private static FontFamily LoadFontFamily(string name)
    {
        try { return new FontFamily(name); }
        catch (ArgumentException) { return FontFamily.GenericSansSerif; }
    }

    public static class Accent
    {
        public static readonly Color CapsLock = Color.FromArgb(22, 163, 74);   // green
        public static readonly Color NumLock = Color.FromArgb(37, 99, 235);    // blue
        public static readonly Color ScrollLock = Color.FromArgb(217, 119, 6); // amber
    }

    /// <summary>The rounded "key cap" drawn in the tray and in the popup.</summary>
    public static class Badge
    {
        public const FontStyle GlyphStyle = FontStyle.Bold;
        public const float GlyphHeightRatio = 0.72f; // glyph em size / badge height
        public const float CornerRatio = 0.24f;      // corner radius / badge width
        public const float OutlineRatio = 1f / 13f;  // OFF outline width / badge width
        public const float MinOutlineWidth = 1.2f;
        public static readonly Color GlyphOnColor = Color.White;
    }

    public static class Tray
    {
        public const int MinIconSize = 16;
        public static readonly Color OffOnLightTaskbar = Color.FromArgb(75, 85, 99);
        public static readonly Color OffOnDarkTaskbar = Color.FromArgb(156, 163, 175);

        public static Color OffColor(bool lightTaskbar) => lightTaskbar ? OffOnLightTaskbar : OffOnDarkTaskbar;
    }

    public static class Osd
    {
        public static readonly Color Background = Color.FromArgb(32, 32, 32);
        public static readonly Color Text = Color.White;
        public static readonly Color OffText = Tray.OffOnDarkTaskbar;

        public const int Width = 240;
        public const int Height = 60;
        public const int CornerRadius = 12;
        public const int BottomMargin = 72;
        public const int PaddingLeft = 14;
        public const int PaddingRight = 18;
        public const int BadgeSize = 34;
        public const int BadgeGap = 12;

        public const float NameFontSize = 17;
        public const FontStyle NameFontStyle = FontStyle.Regular;
        public const float StateFontSize = 16;
        public const FontStyle StateFontStyle = FontStyle.Bold;

        public const double Opacity = 0.94;
        public const double FadeStep = 0.07;
        public const int HoldMilliseconds = 1300;
        public const int FadeIntervalMilliseconds = 15;
    }
}
