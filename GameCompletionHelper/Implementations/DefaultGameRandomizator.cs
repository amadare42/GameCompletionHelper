using System.Collections.Generic;
using System.Linq;
using GameCompletionHelper.Interfaces;

namespace GameCompletionHelper.Implementations
{
    public class DefaultGameRandomizator : IGameRandomizator
    {
        public IGame GetGame(IEnumerable<IGame> games)
        {
            return games.OrderBy(game => game.LastLaunched).FirstOrDefault();
        }
    }
}