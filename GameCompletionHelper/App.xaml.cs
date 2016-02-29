using ProcessWatch;
using System.Runtime.InteropServices;
using System.Windows;
using System.Xml.Serialization;

namespace GameCompletionHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ProcessHook.Instanse.Dispose();
            base.OnExit(e);
        }
    }
}
