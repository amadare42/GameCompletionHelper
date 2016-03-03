using System;
using System.Management;
using System.Diagnostics;
using System.ComponentModel;

namespace ProcessWatch
{
    //todo: rewrite to non-singleton class
    public class ProcessHook : IDisposable
    {
        static private ProcessHook hook;
        public static ProcessHook Instanse
        {
            get
            {
                if (hook == null)
                    hook = new ProcessHook();
                return hook;
            }
        }

        public event EventHandler<ProcessStartEventArgs> ProcessStarted;
        public event EventHandler<ProcessStopEventArgs> ProcessStopped;

        private ManagementEventWatcher StartWatch;
        private ManagementEventWatcher StopWatch;

        public bool IsHooked { get; private set; }

        private ProcessHook()
        {
            StartWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            StopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            StartWatch.EventArrived += new EventArrivedEventHandler(startWatch_EventArrived);
            StopWatch.EventArrived += new EventArrivedEventHandler(stopWatch_EventArrived);
        }

        public void StartHooking()
        {
            if (IsHooked)
                return;

            StartWatch.Start();
            StopWatch.Start();
        }

        public void StopHooking()
        {
            if (!IsHooked)
                return;

            StartWatch.Stop();
            StopWatch.Stop();
        }

        void stopWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (ProcessStopped == null)
                return;

            var processId = (int)(uint)e.NewEvent.Properties["ProcessID"].Value;
            ProcessStopped(sender, new ProcessStopEventArgs(processId, (string)e.NewEvent.Properties["ProcessName"].Value));
        }

        void startWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (ProcessStarted == null)
                return;

            var processId = (int)(uint)e.NewEvent.Properties["ProcessID"].Value;
            try
            {
                var process = Process.GetProcessById(processId);
                if (!process.HasExited)
                {
                    ProcessStarted(sender, new ProcessStartEventArgs(process.MainModule.FileName, process.Id));
                }
            }
            catch (InvalidOperationException) { }
            catch (ArgumentException) { }
            catch (Win32Exception) { }
        }

        public void Dispose()
        {
            StopWatch.Stop();
            StartWatch.Stop();
            StartWatch.Dispose();
            StopWatch.Dispose();
        }
    }

    public class ProcessStopEventArgs : EventArgs
    {
        public readonly int ProcessId;
        public readonly string ProcessName;

        public ProcessStopEventArgs(int id, string name)
        {
            ProcessId = id;
            ProcessName = name;
        }
    }

    public class ProcessStartEventArgs : EventArgs
    {
        public string FileName { get; private set; }
        public int Id { get; private set; }
        public ProcessStartEventArgs(string fileName, int id)
        {
            this.Id = id;
            this.FileName = fileName;
        }        
    }
}
