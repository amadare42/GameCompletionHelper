using GameCompletionHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCompletionHelper
{
    public interface IGameViewModelFactory
    {
        GameViewModel CreateGameViewModel(IGame game);
        GameViewModel CreateDefaultGaveViewModel();
    }
}
