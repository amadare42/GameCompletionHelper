using System;

namespace ProcessWatch
{
    public interface ITrackableProgram
    {
        string Path { get; }
        void Start();
        void Start(DateTime startTime);
        void Stop(); 
    }
}
