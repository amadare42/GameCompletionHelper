using GameCompletionHelper.Model;
using System.Collections.Generic;

namespace GameCompletionHelper
{
    public interface IGamesProvider
    {
        IEnumerable<Game> GetGames();
        void SaveGames(IEnumerable<Game> games);
    }
}
