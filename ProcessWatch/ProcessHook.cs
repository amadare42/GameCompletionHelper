using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;

namespace ProcessWatch
{
    public class ProcessHook : IProcessNotifier
    {
        public event EventHandler<ProcessStartEventArgs> ProcessStarted;

        public event EventHandler<ProcessStopEventArgs> ProcessStopped;

        private ManagementEventWatcher StartWatch;
        private ManagementEventWatcher StopWatch;

        public bool IsHooked { get; private set; }

        public ProcessHook()
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
            IsHooked = true;
        }

        public void StopHooking()
        {
            if (!IsHooked)
                return;

            StartWatch.Stop();
            StopWatch.Stop();
            IsHooked = false;
        }

        private void stopWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (ProcessStopped == null)
                return;
            var time = DateTime.Now;

            var processId = (int)(uint)e.NewEvent.Properties["ProcessID"].Value;
            ProcessStopped(sender, new ProcessStopEventArgs(processId, (string)e.NewEvent.Properties["ProcessName"].Value, time));
        }

        private void startWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (ProcessStarted == null)
                return;

            var processId = (int)(uint)e.NewEvent.Properties["ProcessID"].Value;
            try
            {
                var process = Process.GetProcessById(processId);
                var args = new ProcessStartEventArgs(WinApi.GetExecutablePathAboveVista(processId), processId, process.StartTime);
                ProcessStarted(sender, args);
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

    //todo: merge event args
    public class ProcessStopEventArgs : EventArgs
    {
        public readonly int ProcessId;
        public readonly string ProcessName;
        public readonly DateTime endTime;

        public ProcessStopEventArgs(int id, string name, DateTime time)
        {
            ProcessId = id;
            ProcessName = name;
            endTime = time;
        }
    }

    public class ProcessStartEventArgs : EventArgs
    {
        public string FileName { get; private set; }
        public int Id { get; private set; }
        public DateTime StartTime { get; private set; }

        public ProcessStartEventArgs(string fileName, int id, DateTime startTime)
        {
            this.Id = id;
            this.FileName = fileName;
            this.StartTime = startTime;
        }
    }
}