using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace ProcessWatch
{
    public class ProcessTracker
    {
        private Dictionary<int, string> trackedProcessesPaths = new Dictionary<int, string>();
        public List<ITrackableProgram> TrackedPrograms;

        public ProcessTracker()
        {
            TrackedPrograms = new List<ITrackableProgram>();
            ProcessHook.Instanse.ProcessStarted += ProcessHook_ProcessStarted;
            ProcessHook.Instanse.ProcessStopped += ProcessHook_ProcessStopped;
            if (!ProcessHook.Instanse.IsHooked)
            {
                ProcessHook.Instanse.StartHooking();
            }
        }

        public void CheckRunningProcesses()
        {
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    var fileName = process.MainModule.FileName;
                    var id = process.Id;
                    var trackedPrograms = TrackedPrograms.Where(prog => 
                                                                    string.Compare(prog.Path, fileName, true) == 0
                                                                    && !trackedProcessesPaths.ContainsKey(id)).ToList();

                    if (trackedPrograms.Count > 0)
                    {
                        //getting all process data before adding it to dictionary to prevent issues
                        //when process in exited during its processing
                        var startTime = process.StartTime;

                        trackedProcessesPaths.Add(id, fileName);
                        trackedPrograms.ForEach(prog => prog.Start(startTime));
                    }
                }
                catch (Exception e) when (e is NotSupportedException || e is Win32Exception || e is PlatformNotSupportedException || e is InvalidOperationException) {
                    //empty, just process other process :)
                }
            }
        }

        private void ProcessHook_ProcessStopped(object sender, ProcessStopEventArgs e)
        {
            string path;
            if (trackedProcessesPaths.TryGetValue(e.ProcessId, out path))
            {
                var tracked = TrackedPrograms.Where(prog => string.Compare(prog.Path, path, true) == 0).ToList();
                trackedProcessesPaths.Remove(e.ProcessId);
                if (trackedProcessesPaths.ContainsValue(path))
                {
                    //if program enters this block then there is more than one instance
                    //of program running
                    return;
                }

                if (tracked.Count > 0)
                    tracked.ForEach(prog => prog.Stop());
            }
        }

        private void ProcessHook_ProcessStarted(object sender, ProcessStartEventArgs e)
        {
            var tracked = TrackedPrograms.Where(prog => string.Compare(prog.Path, e.FileName, true) == 0).ToList();
            if (tracked.Count > 0)
            {
                trackedProcessesPaths.Add(e.Id, e.FileName);
                tracked.ForEach(prog => prog.Start());
            }
        }

    }
}
