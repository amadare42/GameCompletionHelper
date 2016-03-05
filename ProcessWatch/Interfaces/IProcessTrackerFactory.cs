using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessWatch.Interfaces
{
    internal interface IProcessTrackerFactory
    {
        IProcessTracker CreateTracker(IEnumerable<ITrackableProgram> trackedPrograms = null);
    }
}