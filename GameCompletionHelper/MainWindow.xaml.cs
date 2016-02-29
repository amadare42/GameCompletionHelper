using GameCompletionHelper.GameProviders;
using GameCompletionHelper.ViewModel;
using System.Windows;

namespace GameCompletionHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var gameViewModelFactory = new DefaultGameViewModelFactory(new DefaultGameSessionFactory());
            var viewModel = new MainViewModel(new FileSystemGameProvider(), new DefaultGameTracker(), new DefaultGameRandomizator(), gameViewModelFactory);
            this.DataContext = viewModel;
        }
    }
}
