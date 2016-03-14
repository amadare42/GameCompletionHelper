using System.Collections.Generic;

namespace ProcessWatch.Interfaces
{
    internal interface IProcessesProvider
    {
        IEnumerable<IProcessInfo> GetProcesses();
    }
}