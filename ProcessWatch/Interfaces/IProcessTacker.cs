using System.Collections.Generic;

namespace ProcessWatch
{
    public interface IProcessTracker
    {
        void AddProgram(ITrackableProgram program);

        void AddPrograms(IEnumerable<ITrackableProgram> programs);

        void RemoveProgram(ITrackableProgram program);

        void UpdateProcesses(IEnumerable<ITrackableProgram> newPrograms = null);
    }
}