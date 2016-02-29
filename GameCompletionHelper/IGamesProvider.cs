using GameCompletionHelper.Model;
using System.Collections.Generic;

namespace GameCompletionHelper
{
    public interface IGamesProvider
    {
        IEnumerable<IGame> GetGames();
        void SaveGames(IEnumerable<IGame> games);
    }
}
