using ProcessWatch.Interfaces;
using System;
using System.Collections.Generic;

namespace ProcessWatch
{
    internal class ProcessTrackerSource<TProcessNotifier, TActiveWindowChangedNotifier>
            : IProgramTrackerSource
        where TProcessNotifier : IProcessNotifier, IControllableHook
        where TActiveWindowChangedNotifier : IActiveWindowNotifier, IControllableHook
    {
        private IProcessesProvider processProvider;
        private TProcessNotifier processNotifier;
        private TActiveWindowChangedNotifier activeWindowNotifier;

        internal ProcessTrackerSource(IProcessesProvider processProvider, TProcessNotifier processNotifier, TActiveWindowChangedNotifier activeWindowNotifier)
        {
            this.processProvider = processProvider;
            this.processNotifier = processNotifier;
            this.activeWindowNotifier = activeWindowNotifier;
        }

        public IProgramTracker CreateTracker(IEnumerable<ITrackableProgram> trackedPrograms = null)
        {
            return new ProgramTracker(processProvider, processNotifier, activeWindowNotifier, trackedPrograms);
        }

        public void StartHook()
        {
            this.processNotifier.StartHook();
            this.activeWindowNotifier.StartHook();
        }

        public void PauseHook()
        {
            this.processNotifier.PauseHook();
            this.activeWindowNotifier.PauseHook();
        }

        public void Dispose()
        {
            var dis = this.processNotifier as IDisposable;
            dis?.Dispose();
            this.activeWindowNotifier.PauseHook();
        }
    }
}