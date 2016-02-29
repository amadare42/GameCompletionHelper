﻿using GameCompletionHelper.Model;
using System;
using System.Collections.Generic;

namespace GameCompletionHelper
{
    public interface IGame
    {
        string PathToExe { get; set; }
        TimeSpan PlayedTotal { get; }
        DateTime LastLaunched { get; }
        IEnumerable<GameSession> Sessions { get; }
        string Name { get; set; }

        void AddSession(GameSession session);
    }
}
