using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessWatch
{
    internal class ProcessNotifier : IProcessNotifier
    {
        public event EventHandler<ProcessStartEventArgs> ProcessStarted;

        public event EventHandler<ProcessStopEventArgs> ProcessStopped;

        public ProcessNotifier(ProcessHook hook)
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}