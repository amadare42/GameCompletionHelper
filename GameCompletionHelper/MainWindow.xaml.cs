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
            var viewModel = new MainViewModel(new FileSystemGameProvider(), new DefaultGameTracker(), new DefaultGameRandomizator());
            this.DataContext = viewModel;
        }
    }
}
