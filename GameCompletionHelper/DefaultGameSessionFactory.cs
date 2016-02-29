using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCompletionHelper.Model;

namespace GameCompletionHelper
{
    class DefaultGameSessionFactory : IGameSessionFactory
    {
        public GameSession CreateGameSession()
        {
            return new GameSession()
            {
                SessionStart = DateTime.Now
            };
        }
    }
}
