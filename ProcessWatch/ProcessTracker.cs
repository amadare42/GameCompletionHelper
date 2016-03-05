using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ProcessWatch
{
    internal class ProcessTracker : IProcessTracker
    {
        private Dictionary<int, string> processIdPathDictonary = new Dictionary<int, string>();

        private List<ITrackableProgram> trackedPrograms;
        private IProcessesProvider processProvider;

        private Dictionary<int, string> processesIds = new Dictionary<int, string>();
        private HashSet<string> processPaths = new HashSet<string>();

        public ProcessTracker(IProcessesProvider processProvider, IProcessNotifier notifier, IEnumerable<ITrackableProgram> trackedPrograms = null)
        {
            this.trackedPrograms = new List<ITrackableProgram>(
                trackedPrograms ?? new ITrackableProgram[0]);
            this.processProvider = processProvider;

            this.AttachNotifyEvents(notifier);
            if (trackedPrograms != null)
            {
                this.UpdateProcesses(this.trackedPrograms);
            }
        }

        private void AttachNotifyEvents(IProcessNotifier notifier)
        {
            notifier.ProcessStarted += OnProcessStarted;
            notifier.ProcessStopped += OnProcessStopped;
        }

        public void UpdateProcesses(IEnumerable<ITrackableProgram> newPrograms = null)
        {
            newPrograms = newPrograms ?? new ITrackableProgram[0];

            Dictionary<int, string> runningProcessIds = new Dictionary<int, string>();
            HashSet<string> runningProcessPaths = new HashSet<string>();

            var programTimes = new Dictionary<ITrackableProgram, List<DateTime>>();
            foreach (ITrackableProgram tprog in this.trackedPrograms)
                programTimes.Add(tprog, new List<DateTime>());

            foreach (var process in processProvider.GetProcesses())
            {
                try
                {
                    var processId = process.Id;
                    var processPath = process.Path.ToLower();
                    var isNewProgram = newPrograms.Any(p => p.Path.Equals(processPath, StringComparison.CurrentCultureIgnoreCase));
                    runningProcessIds[processId] = processPath;
                    runningProcessPaths.Add(processPath);

                    if (!this.processesIds.ContainsKey(processId) || isNewProgram)
                    {
                        var startTime = process.StartTime;

                        foreach (var program in trackedPrograms)
                        {
                            if (processPath.Equals(program.Path, StringComparison.InvariantCultureIgnoreCase))
                            {
                                programTimes[program].Add(startTime);
                            }
                        }
                    }
                }
                catch (Win32Exception e)
                {
                    //empty, just process other process :)
                }
            }

            foreach (var prog in this.trackedPrograms)
            {
                var wasRunning = processPaths.Contains(prog.Path.ToLower());
                var nowRunning = runningProcessPaths.Contains(prog.Path.ToLower());

                if (wasRunning && !nowRunning)
                    prog.Stop();

                if ((!nowRunning && wasRunning)
                    || (newPrograms.Contains(prog) && programTimes[prog].Count > 0))
                    prog.Start(programTimes[prog].Min());
            }

            processesIds = runningProcessIds;
            processPaths = runningProcessPaths;
        }

        public void AddProgram(ITrackableProgram program)
        {
            this.trackedPrograms.Add(program);
            this.UpdateProcesses(new ITrackableProgram[] { program });
        }

        public void AddPrograms(IEnumerable<ITrackableProgram> programs)
        {
            var programList = programs.ToList();
            this.trackedPrograms.AddRange(programList);
            this.UpdateProcesses(programList);
        }

        public void RemoveProgram(ITrackableProgram program)
        {
            this.trackedPrograms.Remove(program);
        }

        private void OnProcessStopped(object sender, ProcessStopEventArgs e)
        {
            string path;
            if (!this.processesIds.TryGetValue(e.ProcessId, out path))
                return;

            this.processesIds.Remove(e.ProcessId);
            var subscribers = this.trackedPrograms.Where(prog => prog.Path.Equals(path, StringComparison.CurrentCultureIgnoreCase)).ToList();

            if (!this.processesIds.ContainsValue(path))
            {
                this.processPaths.Remove(path);

                if (subscribers.Count > 0)
                    subscribers.ForEach(s => s.Stop());
            }
        }

        private void OnProcessStarted(object sender, ProcessStartEventArgs e)
        {
            this.processesIds[e.Id] = e.FileName;
            if (!this.processPaths.Contains(e.FileName))
            {
                foreach (var prog in this.trackedPrograms)
                {
                    if (prog.Path.Equals(e.FileName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        prog.Start(e.StartTime);
                    }
                }
                this.processPaths.Add(e.FileName.ToLower());
            }
        }
    }
}