using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessWatch
{
    interface IProcessInfo
    {
        int Id { get; }
        string Path { get; }
        DateTime StartTime { get; }
    }
}
