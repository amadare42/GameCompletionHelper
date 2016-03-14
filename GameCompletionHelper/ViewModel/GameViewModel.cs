using GameCompletionHelper.Formatters;
using GameCompletionHelper.Helpers;
using GameCompletionHelper.Interfaces;
using GameCompletionHelper.Model;
using GameCompletionHelper.ViewModel.Enums;
using GameCompletionHelper.Views;
using Microsoft.Win32;
using ProcessWatch;
using ProcessWatch.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GameCompletionHelper.ViewModel
{
    public class GameViewModel : BaseViewModel, IGame, ITrackableProgram
    {
        public readonly IGame game;
        private readonly bool InDesign;

        private IGameSessionFactory sessionFactory;

        public ImageSource GameIcon
        {
            get
            {
                if (File.Exists(game.PathToExe))
                {
                    try
                    {
                        Bitmap bmpSource = Icon.ExtractAssociatedIcon(game.PathToExe).ToBitmap();
                        return BitmapHelper.BitmapToImageSource(bmpSource);
                    }
                    catch { }
                }
                return BitmapHelper.NoIconImage;
            }
        }

        private bool isActive = false;

        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                var changed = isActive != value;
                if (changed)
                {
                    isActive = value;
                    OnPropertyChanged();
                    UpdateGameState();
                }
            }
        }

        public TimeSpan AverageSessionSpan
        {
            get
            {
                long longAverageTicks;
                if (this.game.Sessions.Count() > 0)
                {
                    double doubleAverageTicks = this.game.Sessions.Average(session => session.TimePlayedTicks);
                    longAverageTicks = Convert.ToInt64(doubleAverageTicks);
                }
                else
                {
                    longAverageTicks = 0;
                }

                return new TimeSpan(longAverageTicks);
            }
        }

        private GameSession currentSession = null;

        public TimeSpan CurrentSessionSpan
        {
            get
            {
                if (currentSession == null)
                {
                    return default(TimeSpan);
                }
                return currentSession.TimePlayed;
            }
        }

        private bool isOpened;

        public bool IsOpened
        {
            get
            {
                return isOpened;
            }
            set
            {
                isOpened = value;
                OnPropertyChanged();
                UpdateGameState();
            }
        }

        public bool FileExists
        {
            get
            {
                return File.Exists(PathToExe);
            }
        }

        private GameSession selectedGameSession;

        public GameSession SelectedGameSession
        {
            get { return selectedGameSession; }
            set
            {
                selectedGameSession = value;
                OnPropertyChanged();
            }
        }

        private GameState gameState;

        public GameState GameState
        {
            get { return gameState; }
            set
            {
                var changed = gameState != value;
                if (changed)
                {
                    gameState = value;
                    OnPropertyChanged();
                }
            }
        }

        #region ctors

        /// <summary>
        /// Should be used ONLY in design-time by wpf.
        /// </summary>
        public GameViewModel()
        {
            game = Game.Empty;
        }

        public GameViewModel(IGame game, IGameSessionFactory sessionFactory)
        {
            InDesign = false;
            this.game = game;
            this.sessionFactory = sessionFactory;
            AttachToGameEvents();
            this.Sessions = new ObservableCollection<GameSession>(this.game.Sessions);
        }

        #endregion ctors

        #region IGame

        public string PathToExe
        {
            get
            {
                return game.PathToExe;
            }
            set
            {
                if (IsOpened)
                    return;
                var existed = File.Exists(PathToExe);
                var changed = game.PathToExe != value;
                game.PathToExe = value;
                OnPropertyChanged();
                if (changed)
                {
                    OnPropertyChanged(nameof(GameIcon));
                    this.UpdateGameState();
                    if (existed != File.Exists(PathToExe))
                    {
                        OnPropertyChanged(nameof(FileExists));
                    }
                }
            }
        }

        public TimeSpan PlayedTotal
        {
            get
            {
                //todo: add caching
                return game.PlayedTotal;
            }
        }

        public string LastLaunchedFormatted
        {
            get
            {
                return DateTimeFormatter.ConvertToString(this.LastLaunched, !this.IsOpened);
            }
        }

        public DateTime LastLaunched
        {
            get
            {
                //todo: add caching
                return game.LastLaunched;
            }
        }

        public Options Options
        {
            get
            {
                return this.game.Options;
            }

            set
            {
                this.game.Options = value;
            }
        }

        IEnumerable<GameSession> IGame.Sessions
        {
            get
            {
                return game.Sessions;
            }
        }

        private ObservableCollection<GameSession> sessions;

        public ObservableCollection<GameSession> Sessions
        {
            get
            {
                return sessions;
            }

            set
            {
                sessions = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return game.Name;
            }
            set
            {
                game.Name = value;
                OnPropertyChanged();
            }
        }

        public void AddSession(GameSession session)
        {
            this.game.AddSession(session);
        }

        public void RemoveSession(GameSession session)
        {
            this.game.RemoveSession(session);
        }

        public GameSession GetSessionAt(DateTime startTime)
        {
            return this.game.GetSessionAt(startTime);
        }

        public event EventHandler SessionsChanged;

        #endregion IGame

        #region ITrackableProgram

        private bool wasMinimized = false;

        string ITrackableProgram.Path
        {
            get
            {
                return game.PathToExe;
            }
        }

        public async void Start(DateTime startTime)
        {
            //todo: track all multi-instance programs, so after
            //restart tracker willn't add existed programs
            if (IsOpened)
                return;

            var existedSession = this.GetSessionAt(startTime);
            this.currentSession = existedSession ?? sessionFactory.CreateGameSession(startTime);
            if (existedSession == null)
            {
                this.AddSession(currentSession);
            }
            this.OnPropertyChanged(nameof(LastLaunchedFormatted));
            this.OnPropertyChanged(nameof(LastLaunched));
            this.IsOpened = true;
            var inactiveTime = 0;
            while (this.IsOpened)
            {
                await Task.Delay(500);
                //todo: accurate active time calculation
                if (!this.IsActive && this.Options.CalcOnlyOnActive)
                {
                    inactiveTime += 500;
                    continue;
                }
                this.currentSession.TimePlayed = DateTime.Now - this.currentSession.SessionStart - TimeSpan.FromMilliseconds(inactiveTime);
                this.OnPropertyChanged(nameof(CurrentSessionSpan));
                this.OnPropertyChanged(nameof(PlayedTotal));
            }
            if (this.currentSession.TimePlayed.Seconds < 2)
            {
                this.RemoveSession(this.currentSession);
                this.OnPropertyChanged(nameof(PlayedTotal));
                this.OnPropertyChanged(nameof(LastLaunchedFormatted));
                this.OnPropertyChanged(nameof(LastLaunched));
            }
            this.OnPropertyChanged(nameof(AverageSessionSpan));
        }

        //todo: stop on time
        public void Stop()
        {
            IsOpened = false;
            if (this.wasMinimized)
                WinApi.RestoreAll();
            this.wasMinimized = false;
        }

        public void Activate()
        {
            this.IsActive = true;
        }

        public void Deactivate()
        {
            this.IsActive = false;
        }

        #endregion ITrackableProgram

        #region Commands

        public RelayCommand RunCommand
        {
            get
            {
                return new RelayCommand(o => this.Run());
            }
        }

        public RelayCommand ShowOptionsCommand
        {
            get
            {
                return new RelayCommand(o => this.ShowOptions());
            }
        }

        public RelayCommand ShowInExplorerCommand
        {
            get
            {
                return new RelayCommand(e => ShowInExplorer());
            }
        }

        public RelayCommand SelectFileCommand
        {
            get
            {
                return new RelayCommand(e => SelectFile());
            }
        }

        #endregion Commands

        #region Command methods

        public void Run()
        {
            //todo: implement run as admin
            /*Process process = new Process();
            process.StartInfo.FileName = string.IsNullOrEmpty(RunPath) ? this.PathToExe : RunPath;
            if (this.RunAsAdmin)
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.Verb = "runas";
            }
            process.Start();*/
            var runPath = this.game.Options.RunPath;
            var minimize = this.game.Options.MinimizeWindowsOnStart;

            if (minimize)
            {
                WinApi.MinimizeAll();
                wasMinimized = true;
            }
            Process.Start(string.IsNullOrEmpty(runPath) ? this.PathToExe : runPath);
        }

        private void ShowOptions()
        {
            var viewModel = new OptionsViewModel(this.game.Options);
            var window = new GameOptionsWindow(viewModel);
            window.Title = this.Name + " - options";
            window.DataContext = viewModel;
            window.Show();
        }

        public void ShowInExplorer()
        {
            string args = string.Format("/e, /select, \"{0}\"", this.PathToExe);

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "explorer";
            info.Arguments = args;
            Process.Start(info);
        }

        private void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executables (*.exe)|*.exe";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                this.PathToExe = openFileDialog.FileName;
            }
        }

        #endregion Command methods

        private void AttachToGameEvents()
        {
            this.game.SessionsChanged += (sender, args) =>
            {
                Sessions = new ObservableCollection<GameSession>(((IGame)this).Sessions);
                this.OnPropertyChanged(nameof(this.Sessions));
            };
        }

        private void UpdateGameState()
        {
            if (this.InDesign)
                return;

            if (!this.FileExists)
            {
                this.GameState = GameState.InvalidPath;
            }
            else
            {
                if (!this.IsOpened)
                {
                    this.GameState = GameState.ValidNotLaunched;
                }
                else
                {
                    if (this.IsActive)
                    {
                        this.GameState = GameState.LaunchedActive;
                    }
                    else
                    {
                        this.GameState = GameState.LaunchedNotActive;
                    }
                }
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}