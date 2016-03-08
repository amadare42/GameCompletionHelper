using System;

namespace ProcessWatch
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