using GameCompletionHelper.Model;
using GameCompletionHelper.Properties;
using GameCompletionHelper.Views;
using Microsoft.Win32;
using ProcessWatch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameCompletionHelper.ViewModel
{
    public class GameViewModel : BaseViewModel, IGame, ITrackableProgram
    {
        public IGame game;

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

        private bool isActive = true;

        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                var changed = isActive == value;
                isActive = value;
                if (changed)
                    OnPropertyChanged();
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
            }
        }

        public bool FileExists
        {
            get
            {
                return File.Exists(PathToExe);
            }
        }

        #region ctors

        internal GameViewModel()
        {
            game = Game.Empty;
        }

        public GameViewModel(IGame game, IGameSessionFactory sessionFactory)
        {
            this.game = game;
            this.sessionFactory = sessionFactory;
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
                    OnPropertyChanged("GameIcon");
                    if (existed != File.Exists(PathToExe))
                    {
                        OnPropertyChanged("FileExists");
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

        #endregion IGame

        #region ITrackableProgram

        private GameSession currentSession = null;

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
            //todo: fix multiinstance sesion management
            if (IsOpened)
                return;

            var existedSession = this.GetSessionAt(startTime);
            this.currentSession = existedSession ?? sessionFactory.CreateGameSession(startTime);
            if (existedSession == null)
            {
                this.AddSession(currentSession);
            }
            this.OnPropertyChanged(nameof(LastLaunched));
            this.IsOpened = true;
            while (this.IsOpened)
            {
                await Task.Delay(500);
                if (!this.IsActive)
                    continue;
                this.currentSession.TimePlayed = DateTime.Now - this.currentSession.SessionStart;
                this.OnPropertyChanged(nameof(PlayedTotal));
            }
            if (this.currentSession.TimePlayed.Seconds < 2)
            {
                this.RemoveSession(this.currentSession);
                this.OnPropertyChanged(nameof(PlayedTotal));
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

        public override string ToString()
        {
            return this.Name;
        }
    }
}