using System;

namespace ProcessWatch
{
    internal interface IProcessInfo
    {
        int Id { get; }
        string Path { get; }
        DateTime StartTime { get; }
    }
}