using System;

namespace ProcessWatch.Interfaces
{
    internal interface IActiveWindowNotifier
    {
        event ActiveWindowsChangedDeleg ActiveWindowChanged;

        int GetActiveWindowOwnerProcessId();
    }

    internal delegate void ActiveWindowsChangedDeleg(IntPtr hWnd, int processId);
}