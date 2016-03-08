using System;
using System.Collections.Generic;

namespace ProcessWatch.Interfaces
{
    public interface IProgramTrackerSource : IDisposable
    {
        IProgramTracker CreateTracker(IEnumerable<ITrackableProgram> trackedPrograms = null);

        void StartHook();

        void PauseHook();
    }
}