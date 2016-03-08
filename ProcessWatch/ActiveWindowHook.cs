using System;
using System.Runtime.InteropServices;

namespace ProcessWatch
{
    internal static class ActiveWindowHook
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, int idProcess, int idThread, uint dwflags);

        [DllImport("user32.dll")]
        internal static extern int UnhookWinEvent(IntPtr hWinEventHook);

        internal delegate void WinEventProc(IntPtr hWinEventHook, uint iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private static IntPtr winHook;
        private static WinEventProc listener;

        public static bool Listening = false;

        static public event Action<int> ActiveWindowChanged;

        public static void StartListeningForWindowChanges()
        {
            Listening = true;
            listener = new WinEventProc(EventCallback);
            //setting the window hook
            winHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, listener, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public static void StopListeningForWindowChanges()
        {
            UnhookWinEvent(winHook);
        }

        private static void EventCallback(IntPtr hWinEventHook, uint iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime)
        {
            if (ActiveWindowChanged != null)
            {
                uint id;
                GetWindowThreadProcessId(hWnd, out id);
                ActiveWindowChanged((int)id);
            }
        }
    }
}