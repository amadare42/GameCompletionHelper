using GameCompletionHelper.GameProviders;
using GameCompletionHelper.ViewModel;
using ProcessWatch;
using System.ComponentModel;
using System.Windows;

namespace GameCompletionHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProcessTrackerFactory processTrackerFactory = new ProcessTrackerFactory();

        public MainWindow()
        {
            InitializeComponent();
            var gameViewModelFactory = new DefaultGameViewModelFactory(new DefaultGameSessionFactory());
            var viewModel = new MainViewModel(new FileSystemGameProvider(), processTrackerFactory.CreateTracker(), new DefaultGameRandomizator(), gameViewModelFactory);
            this.DataContext = viewModel;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            processTrackerFactory.Dispose();
            base.OnClosing(e);
        }
    }
}