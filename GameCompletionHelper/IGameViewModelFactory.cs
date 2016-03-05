using GameCompletionHelper.ViewModel;

namespace GameCompletionHelper
{
    public interface IGameViewModelFactory
    {
        GameViewModel CreateGameViewModel(IGame game);

        GameViewModel CreateDefaultGaveViewModel();
    }
}