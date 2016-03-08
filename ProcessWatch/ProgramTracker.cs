using ProcessWatch.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ProcessWatch
{
    //todo: use path-program dictionary instead of grabbing Path from ITrackableProgram
    internal class ProgramTracker : IProgramTracker
    {
        private Dictionary<int, string> processIdPathDictionary = new Dictionary<int, string>();
        private HashSet<string> processPaths = new HashSet<string>();
        private List<ITrackableProgram> trackedPrograms;
        private List<ITrackableProgram> activePrograms;
        private int activeProgramId = -1;

        private IProcessesProvider processProvider;
        private IProcessNotifier processNotifier;
        private IActiveWindowNotifier windowNotifier;

        public ProgramTracker(IProcessesProvider processProvider, IProcessNotifier processNotifier, IActiveWindowNotifier windowNotifier,
                                IEnumerable<ITrackableProgram> trackedPrograms = null)
        {
            this.trackedPrograms = new List<ITrackableProgram>(trackedPrograms ?? new ITrackableProgram[0]);
            this.activePrograms = new List<ITrackableProgram>();
            this.processProvider = processProvider;
            this.windowNotifier = windowNotifier;
            this.processNotifier = processNotifier;

            this.AttachNotifyEvents();
            if (trackedPrograms != null)
            {
                this.UpdateProcesses(this.trackedPrograms);
            }
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
                    var processPath = process.Path.ToLower();
                    var isNewProgram = newPrograms.Any(p => p.Path.Equals(processPath, StringComparison.CurrentCultureIgnoreCase));
                    runningProcessIds[process.Id] = processPath;
                    runningProcessPaths.Add(processPath);

                    //notify if program just started is newly subscribed
                    if (!this.processIdPathDictionary.ContainsKey(process.Id) || isNewProgram)
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

            //sending start/stop to running programs
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

            processIdPathDictionary = runningProcessIds;
            processPaths = runningProcessPaths;
        }

        public void UpdateActiveWindowState()
        {
            int newProgramId = windowNotifier.GetActiveWindowOwnerProcessId();
            UpdateActiveWindowsStateToId(newProgramId);
        }

        private void UpdateActiveWindowsStateToId(int newProgramId)
        {
            //Deactivate old programs
            if (this.activeProgramId != newProgramId && this.activeProgramId != -1)
                this.activePrograms.ForEach(p => p.Deactivate());
            this.activeProgramId = newProgramId;

            //Activate new programs
            string path;
            if (processIdPathDictionary.TryGetValue(newProgramId, out path))
            {
                foreach (var prog in trackedPrograms.Where(p => p.Path.Equals(path, StringComparison.CurrentCultureIgnoreCase)))
                {
                    prog.Activate();
                    activePrograms.Add(prog);
                }
            }
        }

        #region IProgramTracker

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

        public void Update()
        {
            UpdateProcesses();
            UpdateActiveWindowState();
        }

        #endregion IProgramTracker

        private void AttachNotifyEvents()
        {
            processNotifier.ProcessStarted += OnProcessStarted;
            processNotifier.ProcessStopped += OnProcessStopped;
            windowNotifier.ActiveWindowChanged += OnActiveWindowChanged;
        }

        private void DeatachNotifyEvents()
        {
            if (processNotifier != null)
            {
                processNotifier.ProcessStarted -= OnProcessStarted;
                processNotifier.ProcessStopped -= OnProcessStopped;
            }
            if (windowNotifier != null)
            {
                windowNotifier.ActiveWindowChanged -= OnActiveWindowChanged;
            }
        }

        private void OnProcessStopped(object sender, ProcessStopEventArgs e)
        {
            string path;
            if (!this.processIdPathDictionary.TryGetValue(e.ProcessId, out path))
                return;

            this.processIdPathDictionary.Remove(e.ProcessId);
            var subscribers = this.trackedPrograms.Where(prog => prog.Path.Equals(path, StringComparison.CurrentCultureIgnoreCase)).ToList();

            if (!this.processIdPathDictionary.ContainsValue(path))
            {
                this.processPaths.Remove(path);

                if (subscribers.Count > 0)
                    subscribers.ForEach(s => s.Stop());
            }
        }

        private void OnProcessStarted(object sender, ProcessStartEventArgs e)
        {
            this.processIdPathDictionary[e.Id] = e.FileName;

            //todo: add C:\Windows\SysWOW64 support
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

        private void OnActiveWindowChanged(IntPtr hWnd, int processId)
        {
            this.UpdateActiveWindowsStateToId(processId);
        }

        ~ProgramTracker()
        {
            this.DeatachNotifyEvents();
        }
    }
}