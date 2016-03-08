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
                var changed = options.RunPath == value;
                options.RunPath = value;
                if (changed)
                    OnPropertyChanged();
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
                var changed = this.options.RunAsAdmin == value;
                options.RunAsAdmin = value;
                if (changed)
                    OnPropertyChanged();
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
                var changed = options.MinimizeWindowsOnStart == value;
                options.MinimizeWindowsOnStart = value;
                if (changed)
                    OnPropertyChanged();
            }
        }
    }
}