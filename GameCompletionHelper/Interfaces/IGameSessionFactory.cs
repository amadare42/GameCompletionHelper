using System;
using GameCompletionHelper.Model;

namespace GameCompletionHelper.Interfaces
{
    public interface IGameSessionFactory
    {
        GameSession CreateGameSession(DateTime startTime);
    }
}
