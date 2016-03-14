using GameCompletionHelper.Interfaces;
using GameCompletionHelper.Model;
using System;

namespace GameCompletionHelper.Factories
{
    internal class DefaultGameSessionFactory : IGameSessionFactory
    {
        public GameSession CreateGameSession(DateTime startTime)
        {
            return new GameSession()
            {
                SessionStart = startTime
            };
        }
    }
}