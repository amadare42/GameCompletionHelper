using ProcessWatch.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace ProcessWatch
{
    class ActiveWindowHook : IControllableHook, IActiveWindowNotifier
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, int idProcess, int idThread, uint dwflags);

        [DllImport("user32.dll")]
        internal static extern int UnhookWinEvent(IntPtr hWinEventHook);

        internal delegate void WinEventProc(IntPtr hWinEventHook, uint iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private IntPtr winHook;
        private WinEventProc listener;

        #region IActiveWindowChangedNotifier

        public static bool Listening = false;

        public event ActiveWindowsChangedDeleg ActiveWindowChanged;

        #endregion IActiveWindowChangedNotifier

        #region IControllableHook

        public void StartHook()
        {
            Listening = true;
            listener = new WinEventProc(EventCallback);
            winHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, listener, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public void PauseHook()
        {
            Listening = false;
            UnhookWinEvent(winHook);
        }

        #endregion IControllableHook

        private void EventCallback(IntPtr hWinEventHook, uint iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime)
        {
            if (ActiveWindowChanged != null)
            {
                uint id;
                GetWindowThreadProcessId(hWnd, out id);
                ActiveWindowChanged(hWnd, (int)id);
            }
        }

        public int GetActiveWindowOwnerProcessId()
        {
            uint id;
            var hWnd = GetForegroundWindow();
            GetWindowThreadProcessId(hWnd, out id);
            return (int)id;
        }
    }
}