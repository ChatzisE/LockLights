using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using LockLights.Interop;

namespace LockLights.Rendering;

/// <summary>Draws the "key cap" badge used in the tray and in the on-screen popup.</summary>
internal static class KeyBadgeRenderer
{
    /// <summary>ON = filled with the accent color; OFF = outline only.</summary>
    public static void Draw(Graphics g, RectangleF bounds, string glyph, bool on, Color accent, Color offColor)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        Color glyphColor;
        if (on)
        {
            using var path = Shapes.RoundedRect(bounds, bounds.Width * Theme.Badge.CornerRatio);
            using var fill = new SolidBrush(accent);
            g.FillPath(fill, path);
            glyphColor = Theme.Badge.GlyphOnColor;
        }
        else
        {
            float penWidth = Math.Max(Theme.Badge.MinOutlineWidth, bounds.Width * Theme.Badge.OutlineRatio);
            var inset = RectangleF.Inflate(bounds, -penWidth / 2, -penWidth / 2);
            using var path = Shapes.RoundedRect(inset, inset.Width * Theme.Badge.CornerRatio);
            using var pen = new Pen(offColor, penWidth);
            g.DrawPath(pen, path);
            glyphColor = offColor;
        }

        DrawCenteredGlyph(g, bounds, glyph, glyphColor);
    }

    public static Icon CreateIcon(string glyph, bool on, Color accent, Color offColor, int size)
    {
        using var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.Transparent);
            Draw(g, new RectangleF(0, 0, size, size), glyph, on, accent, offColor);
        }

        IntPtr handle = bitmap.GetHicon();
        try
        {
            using var temp = Icon.FromHandle(handle);
            return (Icon)temp.Clone(); // the clone owns its own copy of the handle
        }
        finally
        {
            NativeMethods.DestroyIcon(handle);
        }
    }

    /// <summary>Centers the glyph by its actual outline, not by font metrics, so it looks right at 16 px.</summary>
    private static void DrawCenteredGlyph(Graphics g, RectangleF bounds, string glyph, Color color)
    {
        using var path = new GraphicsPath();
        path.AddString(glyph, Theme.FontFamily, (int)Theme.Badge.GlyphStyle,
            bounds.Height * Theme.Badge.GlyphHeightRatio, PointF.Empty, StringFormat.GenericTypographic);

        RectangleF glyphBounds = path.GetBounds();
        using (var transform = new Matrix())
        {
            transform.Translate(
                bounds.X + (bounds.Width - glyphBounds.Width) / 2 - glyphBounds.X,
                bounds.Y + (bounds.Height - glyphBounds.Height) / 2 - glyphBounds.Y);
            path.Transform(transform);
        }

        using var brush = new SolidBrush(color);
        g.FillPath(brush, path);
    }
}
