using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCompletionHelper
{
    public class DefaultGameRandomizator : IGameRandomizator
    {
        public IGame GetGame(IEnumerable<IGame> games)
        {
            return games.OrderBy(game => game.LastLaunched).FirstOrDefault();
        }
        
    }
}
