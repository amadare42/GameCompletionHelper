using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessWatch
{
    public class SimpleTrackableProgram : ITrackableProgram
    {
        private Action start;
        private Action stop;

        public string Path
        {
            get; private set;
        }

        public void Start(DateTime startTime)
        {
            start();
        }

        public void Stop()
        {
            stop();
        }

        public SimpleTrackableProgram(string path, Action start, Action stop)
        {
            this.Path = path;
            this.start = start;
            this.stop = stop;
        }

        public SimpleTrackableProgram()
        {
        }
    }
}