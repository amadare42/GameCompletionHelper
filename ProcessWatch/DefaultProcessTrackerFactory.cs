using ProcessWatch.Interfaces;
using System;
using System.Collections.Generic;

namespace ProcessWatch
{
    public class ProcessTrackerFactory : IProcessTrackerFactory, IDisposable
    {
        private IProcessesProvider processProvider;
        private IProcessNotifier processNotifier;

        public ProcessTrackerFactory()
        {
            processProvider = new DefaultProcessProvider();
            var hook = new ProcessHook();
            hook.StartHooking();
            processNotifier = hook;
        }

        internal ProcessTrackerFactory(IProcessesProvider processProvider, IProcessNotifier processNotifier)
        {
            this.processProvider = processProvider;
            this.processNotifier = processNotifier;
        }

        public IProcessTracker CreateTracker(IEnumerable<ITrackableProgram> trackedPrograms = null)
        {
            return new ProcessTracker(processProvider, processNotifier, trackedPrograms);
        }

        public void Dispose()
        {
            processNotifier.Dispose();
        }
    }
}