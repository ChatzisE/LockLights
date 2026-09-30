using LockLights.Interop;

namespace LockLights.Input;

internal static class Keyboard
{
    public static bool IsLocked(Keys key) => Control.IsKeyLocked(key);

    /// <summary>Simulates a press and release of a lock key so its state flips.</summary>
    public static void Toggle(Keys key)
    {
        byte vk = (byte)key;
        byte scan = (byte)NativeMethods.MapVirtualKey(vk, NativeMethods.MAPVK_VK_TO_VSC);
        uint flags = key == Keys.NumLock ? NativeMethods.KEYEVENTF_EXTENDEDKEY : 0;

        NativeMethods.keybd_event(vk, scan, flags, UIntPtr.Zero);
        NativeMethods.keybd_event(vk, scan, flags | NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
    }
}
