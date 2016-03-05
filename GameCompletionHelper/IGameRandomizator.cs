using System.Collections.Generic;

namespace GameCompletionHelper
{
    //todo: implement session-based randomizator
    public interface IGameRandomizator
    {
        IGame GetGame(IEnumerable<IGame> games);
    }
}