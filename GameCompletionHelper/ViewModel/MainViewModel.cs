﻿using GameCompletionHelper.Interfaces;
using ProcessWatch;
using ProcessWatch.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GameCompletionHelper.ViewModel
{
    //todo: program options
    /*
        Language
        TimeFormatter
        Multiple game randomizator method
     */

    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<GameViewModel> Games { get; set; }

        private IGamesProvider gamesProvider { get; set; }
        private IProgramTracker gameTracker { get; set; }
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
            this.Games = new ObservableCollection<GameViewModel>()
            {
                new GameViewModel()
                    {
                        Name = "Active game",
                        GameState = Enums.GameState.LaunchedActive
                    },
                new GameViewModel()
                    {
                        Name = "Launched not active game",
                        GameState = Enums.GameState.LaunchedNotActive
                    },
                new GameViewModel()
                    {
                        Name = "Game with invalid path",
                        GameState = Enums.GameState.InvalidPath
                    },
                new GameViewModel()
                    {
                        Name = "Configured game",
                        GameState = Enums.GameState.ValidNotLaunched
                    },
            };
        }

        public MainViewModel(IGamesProvider gamesProvider, IProgramTracker gameTracker, IGameRandomizator gameRandomizator, IGameViewModelFactory gameViewModelFactory)
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

        public RelayCommand UpdateProcessesCommand
        {
            get
            {
                return new RelayCommand(e => UpdateProcesses());
            }
        }

        private void UpdateProcesses()
        {
            this.gameTracker.Update();
        }

        private void RunNextGame(object obj)
        {
            var gameViewModel = ((GameViewModel)this.gameRandomizator.GetGame(this.Games));
            if (gameViewModel == null)
                return;
            gameViewModel.Run();
        }

        private void ShowNextGame(object obj)
        {
            var gameViewModel = ((GameViewModel)this.gameRandomizator.GetGame(this.Games));
            SelectedGame = gameViewModel;
        }

        private void RemoveCurrentGame(object obj)
        {
            var result = MessageBox.Show(string.Format("Are you sure want to delete {0}?", this.SelectedGame.Name), this.SelectedGame.Name, MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes)
            {
                this.gameTracker.RemoveProgram(SelectedGame);
                this.Games.Remove(SelectedGame);
            }
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
                if (game.FileExists)
                {
                    AddGameToTracker(game);
                }
                else
                {
                    this.gameTracker.RemoveProgram(game);
                }
                //game.PropertyChanged -= NewGame_PropertyChanged;
            }
        }

        private void AddGameToTracker(GameViewModel gameViewModel)
        {
            this.gameTracker.AddProgram(gameViewModel);
        }

        public void SaveAll(object o)
        {
            gamesProvider.SaveGames(this.Games);
        }
    }
}