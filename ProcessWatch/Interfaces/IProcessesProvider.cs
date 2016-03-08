using System.Collections.Generic;

namespace ProcessWatch
{
    internal interface IProcessesProvider
    {
        IEnumerable<IProcessInfo> GetProcesses();
    }
}