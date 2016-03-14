using System;

namespace ProcessWatch.Interfaces
{
    internal interface IProcessInfo
    {
        int Id { get; }
        string Path { get; }
        DateTime StartTime { get; }
    }
}