using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessWatch.Interfaces;

namespace ProcessWatch.Tests
{
    public class SimpleTrackableProgram : ITrackableProgram
    {
        public Action start;
        public Action stop;
        public Action activate;
        public Action deactivate;

        public string Path
        {
            get; set;
        }

        public void Start(DateTime startTime)
        {
            start();
        }

        public void Stop()
        {
            stop();
        }

        public void Deactivate()
        {
            if (deactivate != null)
            {
                deactivate();
            }
        }

        public void Activate()
        {
            if (activate != null)
            {
                activate();
            }
        }

        public SimpleTrackableProgram(string path, Action start, Action stop)
        {
            this.Path = path;
            this.start = start;
            this.stop = stop;
        }

        public SimpleTrackableProgram(string path, Action start, Action stop, Action activate, Action deactivate)
        {
            this.Path = path;
            this.start = start;
            this.stop = stop;
            this.activate = activate;
            this.deactivate = deactivate;
        }

        public SimpleTrackableProgram()
        {
        }
    }
}