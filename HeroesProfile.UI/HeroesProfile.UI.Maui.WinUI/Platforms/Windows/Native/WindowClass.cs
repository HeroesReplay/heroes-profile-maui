using System;
using System.Runtime.InteropServices;

using static HeroesProfile.UI.Maui.Platforms.Windows.WinApi;

namespace HeroesProfile.UI.Maui.Platforms.Windows
{
    /// <summary>
    /// Win API WNDCLASS struct - represents a single window.
    /// Used to receive window messages.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowClass
    {
#pragma warning disable 1591

        public uint style;
        public WindowProcedureHandler lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)] public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)] public string lpszClassName;

#pragma warning restore 1591
    }
}