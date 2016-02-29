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

namespace GameCompletionHelper.ViewModel
{
    public class GameViewModel : BaseViewModel, IGame, ITrackableProgram
    {
        public Game game;

        private ImageSourceConverter imageSourceConverter = new ImageSourceConverter();

        public GameViewModel()
        {
            game = new Game();
        }
        public GameViewModel(Game game)
        {
            this.game = game;
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
        public TimeSpan TimePlayed
        {
            get
            {
                return game.TimePlayed;
            }
            set
            {
                game.TimePlayed = value;
                OnPropertyChanged();
            }
        }

        public DateTime LastLaunched
        {
            get
            {
                return game.LastLaunched;
            }
            set
            {
                game.LastLaunched = value;
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
                if (IsOpened)
                {
                    return;
                }
                game.Name = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region ITrackableProgram

        string ITrackableProgram.Path
        {
            get
            {
                return game.PathToExe;
            }
        }

        public async void Start()
        {
            var prevPlayed = TimePlayed;
            LastLaunched = DateTime.Now;
            IsOpened = true;
            while (IsOpened)
            {
                await Task.Delay(1000);
                TimePlayed = DateTime.Now - LastLaunched + prevPlayed;
            }
        }

        public void Stop()
        {
            IsOpened = false;
        }

        #endregion

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
                return new RelayCommand( o => this.Run());
            }
        }

        public void Run()
        {
            Process.Start(this.PathToExe);
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

    }
}
