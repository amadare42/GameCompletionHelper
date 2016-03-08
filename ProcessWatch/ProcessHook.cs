using ProcessWatch.Interfaces;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;

namespace ProcessWatch
{
    public class ProcessHook : IProcessNotifier, IControllableHook, IDisposable
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
            StartWatch.EventArrived += startWatch_EventArrived;
            StopWatch.EventArrived += stopWatch_EventArrived;
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

        public void StartHook()
        {
            StartWatch.Start();
            StopWatch.Start();
        }

        public void PauseHook()
        {
            StopWatch.Stop();
            StartWatch.Stop();
        }

        public void Dispose()
        {
            StopWatch.Stop();
            StartWatch.Stop();
            StartWatch.Dispose();
            StopWatch.Dispose();
        }
    }
}