using System.Windows;

namespace MMK.HotMark.PianoKeyboardUsage
{
    public partial class App 
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = new MainView
            {
                DataContext = new MainViewModel()
            };

            MainWindow.Show();
        }
    }
}
