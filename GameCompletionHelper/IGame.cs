using GameCompletionHelper.Model;
using System;
using System.Collections.Generic;

namespace GameCompletionHelper
{
    public interface IGame
    {
        string PathToExe { get; set; }
        string RunPath { get; set; }
        bool RunAsAdmin { get; set; }
        TimeSpan PlayedTotal { get; }
        DateTime LastLaunched { get; }
        IEnumerable<GameSession> Sessions { get; }
        string Name { get; set; }

        void AddSession(GameSession session);

        void RemoveSession(GameSession session);

        bool HasSessionAt(DateTime startTime);

        GameSession GetSessionAt(DateTime startTime);
    }
}