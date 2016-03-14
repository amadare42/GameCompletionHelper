using System;

namespace ProcessWatch.Interfaces
{
    public interface ITrackableProgram
    {
        string Path { get; }

        void Start(DateTime startTime);

        void Stop();

        void Deactivate();

        void Activate();
    }
}