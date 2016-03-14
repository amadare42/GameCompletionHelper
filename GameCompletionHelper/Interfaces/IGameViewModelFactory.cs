using GameCompletionHelper.ViewModel;

namespace GameCompletionHelper.Interfaces
{
    public interface IGameViewModelFactory
    {
        GameViewModel CreateGameViewModel(IGame game);

        GameViewModel CreateDefaultGaveViewModel();
    }
}