using System;
using System.Runtime.InteropServices;

namespace GameCompletionHelper
{
    public static class WinApi
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);

        public const int WM_COMMAND = 0x111;
        public const int MIN_ALL = 419;
        public const int MIN_ALL_UNDO = 416;
        private const uint SW_RESTORE = 0x09;

        public static void MinimizeAll()
        {
            IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);
            SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
        }

        public static void RestoreAll()
        {
            IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);
            SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL_UNDO, IntPtr.Zero);
        }

        public static void Restore(IntPtr handle)
        {
            ShowWindow(handle, SW_RESTORE);
        }
    }
}