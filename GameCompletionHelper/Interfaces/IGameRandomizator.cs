using System.Collections.Generic;

namespace GameCompletionHelper.Interfaces
{
    //todo: implement session-based randomizator
    public interface IGameRandomizator
    {
        IGame GetGame(IEnumerable<IGame> games);
    }
}