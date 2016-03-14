using GameCompletionHelper.Interfaces;
using GameCompletionHelper.Model;
using GameCompletionHelper.ViewModel;

namespace GameCompletionHelper.Factories
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