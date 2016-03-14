using System;

namespace ProcessWatch.Interfaces
{
    internal interface IProcessNotifier
    {
        event EventHandler<ProcessStartEventArgs> ProcessStarted;

        event EventHandler<ProcessStopEventArgs> ProcessStopped;
    }

    public class ProcessStopEventArgs : EventArgs
    {
        public readonly int ProcessId;
        public readonly string ProcessName;
        public readonly DateTime endTime;

        public ProcessStopEventArgs(int id, string name, DateTime time)
        {
            ProcessId = id;
            ProcessName = name;
            endTime = time;
        }
    }

    public class ProcessStartEventArgs : EventArgs
    {
        public string FileName { get; private set; }
        public int Id { get; private set; }
        public DateTime StartTime { get; private set; }

        public ProcessStartEventArgs(string fileName, int id, DateTime startTime)
        {
            this.Id = id;
            this.FileName = fileName;
            this.StartTime = startTime;
        }
    }
}