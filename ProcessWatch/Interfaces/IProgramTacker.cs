﻿using System.Collections.Generic;

namespace ProcessWatch
{
    public interface IProgramTracker
    {
        void AddProgram(ITrackableProgram program);

        void AddPrograms(IEnumerable<ITrackableProgram> programs);

        void RemoveProgram(ITrackableProgram program);

        void Update();
    }
}