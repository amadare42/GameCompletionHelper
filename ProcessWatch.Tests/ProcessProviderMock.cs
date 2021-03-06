﻿using ProcessWatch;
using ProcessWatch.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessWatch.Tests
{
    internal class ProcessProviderMock : IProcessesProvider, IControllableHook
    {
        public List<IProcessInfo> Processes = new List<IProcessInfo>();

        public ProcessProviderMock(List<IProcessInfo> processes)
        {
            this.Processes = processes;
        }

        public ProcessProviderMock()
        {
        }

        public IEnumerable<IProcessInfo> GetProcesses()
        {
            return Processes;
        }

        public void StartHook()
        {
        }

        public void PauseHook()
        {
        }
    }
}