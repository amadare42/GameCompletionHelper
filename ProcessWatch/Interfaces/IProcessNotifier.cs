using System;

namespace ProcessWatch
{
    internal interface IProcessNotifier : IDisposable
    {
        event EventHandler<ProcessStartEventArgs> ProcessStarted;

        event EventHandler<ProcessStopEventArgs> ProcessStopped;
    }
}