using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessWatch
{
    internal interface IProcessesProvider
    {
        IEnumerable<IProcessInfo> GetProcesses();
    }
}