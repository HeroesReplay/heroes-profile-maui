using HeroesProfile.UI.Services.Tray;
using System.Runtime.InteropServices;
using WinRT.Interop;

public class PlatformTrayService : IPlatformTrayService
{
    private NOTIFYICONDATA notifyIconData;
    private IntPtr originalWndProcPtr;
    private WndProcDelegate? newWndProcDelegate;

    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    static extern bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA pnid);
    
    [DllImport("user32.dll")]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    private const uint NIM_ADD = 0x00000000;
    private const uint NIM_MODIFY = 0x00000001;
    private const uint NIM_DELETE = 0x00000002;
    private const uint NIF_MESSAGE = 0x00000001;
    private const uint NIF_ICON = 0x00000002;
    private const uint NIF_TIP = 0x00000004;
    private const int SW_RESTORE = 9;
    private const int GWL_WNDPROC = -4;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_RBUTTONDOWN = 0x0204;
    private const int WM_TRAYICON = 0x0400 + 1024;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NOTIFYICONDATA
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uID;
        public uint uFlags;
        public uint uCallbackMessage;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
        public uint dwState;
        public uint dwStateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;
        public uint uVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;
        public uint dwInfoFlags;
        public Guid guidItem;
        public IntPtr hBalloonIcon;
    }

    private Microsoft.UI.Xaml.Window MainWindow => Application.Current!.Windows[0]!.Handler!.PlatformView as Microsoft.UI.Xaml.Window;

    public void Initialize()
    {
        var hwnd = WindowNative.GetWindowHandle(MainWindow);

        notifyIconData = new NOTIFYICONDATA
        {
            cbSize = (uint)Marshal.SizeOf(typeof(NOTIFYICONDATA)),
            hWnd = hwnd,
            uID = 0,
            uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP,
            uCallbackMessage = WM_TRAYICON,
            hIcon = LoadIcon("Platforms/Windows/Images/logo.ico"),
            szTip = "Heroes Profile - Uploader"
        };

        Shell_NotifyIcon(NIM_ADD, ref notifyIconData);
        newWndProcDelegate = new WndProcDelegate(WndProc);
        originalWndProcPtr = SetWindowLongPtr(hwnd, GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newWndProcDelegate));
    }

    public Action? ClickHandler { get; set; }

    private IntPtr LoadIcon(string iconPath)
    {
        var path = Path.Combine(AppContext.BaseDirectory, iconPath);
        return System.Drawing.Icon.ExtractAssociatedIcon(path)!.Handle;
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_TRAYICON)
        {
            switch ((int)lParam)
            {
                case WM_LBUTTONDOWN:
                    ReactivateMainWindow();
                    break;
            }
        }
        return CallWindowProc(originalWndProcPtr, hWnd, msg, wParam, lParam);
    }

    private void ReactivateMainWindow()
    {
        var hwnd = WindowNative.GetWindowHandle(MainWindow);
        ShowWindow(hwnd, SW_RESTORE);
        SetForegroundWindow(hwnd);
    }
}