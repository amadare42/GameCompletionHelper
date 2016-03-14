using GameCompletionHelper.Model;

namespace GameCompletionHelper.ViewModel
{
    public class OptionsViewModel : BaseViewModel
    {
        private Options options;

        public OptionsViewModel()
        {
            this.options = new Options();
        }

        public OptionsViewModel(Options options)
        {
            this.options = options;
        }

        public string RunPath
        {
            get
            {
                return options.RunPath;
            }

            set
            {
                var changed = options.RunPath != value;
                if (changed)
                {
                    options.RunPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool RunAsAdmin
        {
            get
            {
                return this.options.RunAsAdmin;
            }

            set
            {
                var changed = this.options.RunAsAdmin != value;
                if (changed)
                {
                    options.RunAsAdmin = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool CalcOnlyOnActive
        {
            get
            {
                return this.options.CalcOnlyOnActive;
            }

            set
            {
                var changed = this.options.CalcOnlyOnActive != value;
                if (changed)
                {
                    options.CalcOnlyOnActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool MinimizeWindowsOnStart
        {
            get
            {
                return options.MinimizeWindowsOnStart;
            }

            set
            {
                var changed = options.MinimizeWindowsOnStart != value;
                if (changed)
                {
                    options.MinimizeWindowsOnStart = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}