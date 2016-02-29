using GameCompletionHelper.Model;

namespace GameCompletionHelper
{
    public class GameEventArgs
    {
        public readonly IGame Game;

        public GameEventArgs(IGame game)
        {
            this.Game = game;
        }
    }
}