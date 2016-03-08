using GameCompletionHelper.GameProviders;
using GameCompletionHelper.ViewModel;
using ProcessWatch;
using ProcessWatch.Interfaces;
using System.ComponentModel;
using System.Windows;

namespace GameCompletionHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IProgramTrackerSource processTrackerSource;

        public MainWindow()
        {
            InitializeComponent();
            processTrackerSource = ProcessTrackerSourceFactory.GetProcessTrackerSource();
            processTrackerSource.StartHook();

            var gameViewModelFactory = new DefaultGameViewModelFactory(new DefaultGameSessionFactory());
            var viewModel = new MainViewModel(new FileSystemGameProvider(), processTrackerSource.CreateTracker(), new DefaultGameRandomizator(), gameViewModelFactory);
            this.DataContext = viewModel;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            processTrackerSource.Dispose();
            base.OnClosing(e);
        }
    }
}