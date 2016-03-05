using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessWatch
{
    internal class TrackedProcessInfo : IProcessInfo
    {
        public int Id
        {
            get;

            private set;
        }

        public string Path
        {
            get;

            private set;
        }

        public DateTime StartTime
        {
            get;

            private set;
        }

        public TrackedProcessInfo(int id, string path, DateTime startTime)
        {
            this.Id = id;
            this.Path = path;
            this.StartTime = startTime;
        }
    }
}