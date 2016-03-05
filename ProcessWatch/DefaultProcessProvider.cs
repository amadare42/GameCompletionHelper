using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace ProcessWatch
{
    internal class DefaultProcessProvider : IProcessesProvider
    {
        public IEnumerable<IProcessInfo> GetProcesses()
        {
            List<TrackedProcessInfo> processesInfo = new List<TrackedProcessInfo>();
            foreach (var proc in Process.GetProcesses())
            {
                try
                {
                    var id = proc.Id;
                    var path = WinApi.GetExecutablePathAboveVista(proc.Id);
                    var startTime = proc.StartTime;
                    processesInfo.Add(new TrackedProcessInfo(id, path, startTime));
                }
                catch (Win32Exception e) { }
            }

            return processesInfo;
        }
    }
}