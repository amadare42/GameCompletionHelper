﻿using System.Collections.Generic;

namespace GameCompletionHelper.Interfaces
{
    public interface IGamesProvider
    {
        IEnumerable<IGame> GetGames();

        void SaveGames(IEnumerable<IGame> games);
    }
}