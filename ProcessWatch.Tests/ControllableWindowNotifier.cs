using ProcessWatch.Interfaces;
using System;

namespace ProcessWatch.Tests
{
    class ControllableWindowNotifier : IActiveWindowNotifier, IControllableHook
    {
        public event ActiveWindowsChangedDeleg ActiveWindowChanged;

        public int ActiveWindowId { get; set; } = 0;

        public int GetActiveWindowOwnerProcessId()
        {
            return ActiveWindowId;
        }

        public void InvokeActiveWindowChanged(int processId, IntPtr hWnd = default(IntPtr))
        {
            ActiveWindowChanged(hWnd, processId);
        }

        public void StartHook()
        {
        }

        public void PauseHook()
        {
        }
    }
}