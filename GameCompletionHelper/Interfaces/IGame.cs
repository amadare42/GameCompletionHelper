using System;
using System.Collections.Generic;
using GameCompletionHelper.Model;

namespace GameCompletionHelper.Interfaces
{
    public interface IGame
    {
        string Name { get; set; }
        Options Options { get; set; }
        string PathToExe { get; set; }
        TimeSpan PlayedTotal { get; }
        DateTime LastLaunched { get; }
        IEnumerable<GameSession> Sessions { get; }

        event EventHandler SessionsChanged;

        void AddSession(GameSession session);

        void RemoveSession(GameSession session);

        GameSession GetSessionAt(DateTime startTime);
    }
}