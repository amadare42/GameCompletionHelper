using System;
using GameCompletionHelper.ViewModel;
using GameCompletionHelper.Model;

namespace GameCompletionHelper
{
    class DefaultGameViewModelFactory : IGameViewModelFactory
    {
        private IGameSessionFactory sessionFactory;

        public DefaultGameViewModelFactory(IGameSessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public GameViewModel CreateDefaultGaveViewModel()
        {
            return new GameViewModel(Game.Empty, sessionFactory);
        }

        public GameViewModel CreateGameViewModel(IGame game)
        {
            return new GameViewModel(game, sessionFactory);
        }
    }
}
