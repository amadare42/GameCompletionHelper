using GameCompletionHelper.Model;
using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using GameCompletionHelper.Properties;
using System.IO;
using ProcessWatch;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using GameCompletionHelper.Views;
using System.Windows;
using System.Text;
using Microsoft.Win32;

namespace GameCompletionHelper.ViewModel
{
    public class GameViewModel : BaseViewModel, IGame, ITrackableProgram
    {
        public IGame game;

        private ImageSourceConverter imageSourceConverter = new ImageSourceConverter();

        public GameViewModel()
        {
            game = Game.Empty;
        }

        public GameViewModel(IGame game, IGameSessionFactory sessionFactory)
        {
            this.game = game;
            this.sessionFactory = sessionFactory;
        }

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
                var changed = game.PathToExe != value;
                game.PathToExe = value;
                OnPropertyChanged();
                if (changed)
                {
                    OnPropertyChanged("GameIcon");
                    OnPropertyChanged("FileExists");
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
                if (IsOpened)
                    return;
                game.Name = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region ITrackableProgram

        GameSession currentSession = null;

        string ITrackableProgram.Path
        {
            get
            {
                return game.PathToExe;
            }
        }

        public async void Start()
        {
            if (IsOpened)
                return;            
            this.currentSession = sessionFactory.CreateGameSession();
            this.AddSession(currentSession);
            this.OnPropertyChanged(nameof(LastLaunched));
            this.IsOpened = true;
            while (this.IsOpened)
            {
                await Task.Delay(1000);
                this.currentSession.TimePlayed = DateTime.Now - this.currentSession.SessionStart;
                this.OnPropertyChanged(nameof(PlayedTotal));
            }
            if (this.currentSession.TimePlayed.Seconds < 10)
            {
                this.RemoveSession(this.currentSession);
                this.OnPropertyChanged(nameof(PlayedTotal));
                this.OnPropertyChanged(nameof(LastLaunched));
            }
            this.OnPropertyChanged(nameof(AverageSessionSpan));
        }

        public void Stop()
        {
            IsOpened = false;
        }

        #endregion

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

        private void ShowOptions()
        {
            var window = new GameOptionsWindow();
            window.DataContext = this;
            window.Show();
        }

        public void Run()
        {
            //todo: implement run as admin
            Process.Start(string.IsNullOrEmpty(RunPath) ? this.PathToExe : RunPath);
        }

        public void ShowInExplorer()
        {
            string args = string.Format("/e, /select, \"{0}\"", this.PathToExe);

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "explorer";
            info.Arguments = args;
            Process.Start(info);
        }

        public ImageSource GameIcon
        {
            get
            {
                if (File.Exists(game.PathToExe))
                {
                    try
                    {
                        Bitmap bmpSource = Icon.ExtractAssociatedIcon(game.PathToExe).ToBitmap();
                        return BitmapToImageSource(bmpSource);
                    }
                    catch { }
                }
                return this.NoIconImage;
            }
        }

        private BitmapImage noIconImage;
        private IGameSessionFactory sessionFactory;

        public BitmapImage NoIconImage
        {
            get
            {
                if (noIconImage == null)
                {
                    var noIcon = Resources.NoIcon;
                    noIcon.MakeTransparent(noIcon.GetPixel(0, 0));
                    noIconImage = BitmapToImageSource(noIcon);

                }
                return noIconImage;
            }
        }

        public bool RunAsAdmin
        {
            get
            {
                return game.RunAsAdmin;
            }
            set
            {
                game.RunAsAdmin = value;
                OnPropertyChanged();
            }
        }

        public string RunPath
        {
            get
            {
                return game.RunPath;
            }
            set
            {
                game.RunPath = value;
                OnPropertyChanged();
            }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void AddSession(GameSession session)
        {
            this.game.AddSession(session);
        }

        public void RemoveSession(GameSession session)
        {
            this.game.RemoveSession(session);
        }
    }
}
