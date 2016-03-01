using GameCompletionHelper.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace GameCompletionHelper.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<GameViewModel> Games { get; set; }

        private IGamesProvider gamesProvider { get; set; }
        private IGameTracker gameTracker { get; set; }
        private IGameRandomizator gameRandomizator { get; set; }

        private bool autoSaving;

        public bool AutoSaving
        {
            get { return autoSaving; }
            set
            {
                if (value == autoSaving)
                    return;
                autoSaving = value;
                if (value)
                    StartAutoSave();
            }
        }


        public MainViewModel()
        {
            this.Games = new ObservableCollection<GameViewModel>();
        }

        public MainViewModel(IGamesProvider gamesProvider, IGameTracker gameTracker, IGameRandomizator gameRandomizator, IGameViewModelFactory gameViewModelFactory)
        {
            this.gamesProvider = gamesProvider;
            this.gameTracker = gameTracker;
            this.gameRandomizator = gameRandomizator;
            this.gameViewModelFactory = gameViewModelFactory;

            //todo: move to loading part
            var games = gamesProvider.GetGames();
            this.Games = new ObservableCollection<GameViewModel>();

            foreach (var game in games.Select(g => this.gameViewModelFactory.CreateGameViewModel(g)))
            {
                this.RegisterGame(game);
            }

            StartAutoSave();
        }

        public async void StartAutoSave()
        {
            autoSaving = true;
            while (autoSaving)
            {
                await Task.Delay(3000);
                gamesProvider.SaveGames(this.Games);
            }
        }

        private GameViewModel selectedGame;
        private IGameViewModelFactory gameViewModelFactory;

        public GameViewModel SelectedGame
        {
            get
            {
                return selectedGame;
            }
            set
            {
                selectedGame = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SaveAllCommand
        {
            get
            {
                return new RelayCommand(this.SaveAll);
            }
        }

        public RelayCommand AddGameCommand
        {
            get
            {
                return new RelayCommand(this.AddGame);
            }
        }

        public RelayCommand RemoveCurrentGameCommand
        {
            get
            {
                return new RelayCommand(this.RemoveCurrentGame);
            }
        }

        public RelayCommand RunNextGameCommand
        {
            get
            {
                return new RelayCommand(this.RunNextGame);
            }
        }

        public RelayCommand ShowNextGameCommand
        {
            get
            {
                return new RelayCommand(this.ShowNextGame);
            }
        }

        private void RunNextGame(object obj)
        {
            var gameViewModel = ((GameViewModel)this.gameRandomizator.GetGame(this.Games));
            gameViewModel.Run();
        }

        private void ShowNextGame(object obj)
        {
            var gameViewModel = ((GameViewModel)this.gameRandomizator.GetGame(this.Games));
            SelectedGame = gameViewModel;
        }

        private void RemoveCurrentGame(object obj)
        {
            this.gameTracker.RemoveGame(SelectedGame);
            this.Games.Remove(SelectedGame);
        }

        public void AddGame(object o)
        {
            var newGame = this.gameViewModelFactory.CreateDefaultGaveViewModel();
            RegisterGame(newGame);
        }

        private void RegisterGame(GameViewModel gameViewModel)
        {
            this.Games.Add(gameViewModel);
            if (gameViewModel.FileExists)
            {
                AddGameToTracker(gameViewModel);
            }
            else
            {
                gameViewModel.PropertyChanged += NewGame_PropertyChanged;
            }
        }

        private void NewGame_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var game = ((GameViewModel)sender);
            if (e.PropertyName == "FileExists" && game.FileExists)
            {
                AddGameToTracker(game);
                game.PropertyChanged -= NewGame_PropertyChanged;
            }
        }

        private void AddGameToTracker(GameViewModel gameViewModel)
        {
            this.gameTracker.AddGame(gameViewModel);
        }

        public void SaveAll(object o)
        {
            gamesProvider.SaveGames(this.Games.Select(g => g.game));
        }
    }
}
