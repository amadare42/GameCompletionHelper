using ProcessWatch;
using ProcessWatch.Interfaces;
using System;

namespace ProcessWatch.Tests
{
    internal class ControllableProcessNotifier : IProcessNotifier, IControllableHook
    {
        public event EventHandler<ProcessStartEventArgs> ProcessStarted;

        public event EventHandler<ProcessStopEventArgs> ProcessStopped;

        public void InvokeProcessStarted(ProcessStartEventArgs startEventArgs)
        {
            if (this.ProcessStarted != null)
            {
                this.ProcessStarted(this, startEventArgs);
            }
        }

        public void InvokeProcessStopped(ProcessStopEventArgs stopEventArgs)
        {
            if (this.ProcessStopped != null)
            {
                this.ProcessStopped(this, stopEventArgs);
            }
        }

        public void Dispose()
        {
        }

        public void StartHook()
        {
        }

        public void PauseHook()
        {
        }
    }
}