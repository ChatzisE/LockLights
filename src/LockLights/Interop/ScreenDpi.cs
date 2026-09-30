namespace LockLights.Interop;

internal static class ScreenDpi
{
    public const int Default = 96;

    /// <summary>Scale factor (1.0 = 100%) of the monitor containing the given physical point.</summary>
    public static float ScaleAt(Point pt) => DpiAt(pt) / (float)Default;

    public static int DpiAt(Point pt)
    {
        try
        {
            IntPtr monitor = NativeMethods.MonitorFromPoint(pt, NativeMethods.MONITOR_DEFAULTTONEAREST);
            if (NativeMethods.GetDpiForMonitor(monitor, NativeMethods.MDT_EFFECTIVE_DPI, out uint dpiX, out _) == 0 && dpiX > 0)
                return (int)dpiX;
        }
        catch (DllNotFoundException) { }
        catch (EntryPointNotFoundException) { }
        return Default;
    }
}
