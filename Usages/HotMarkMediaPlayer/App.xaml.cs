using System.Windows;
using MMK.HotMark.MediaPlayer.Views;
using MMK.HotMark.ViewModels;

namespace MMK.HotMark.MediaPlayer
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            StartupMainView();

            base.OnStartup(e);
        }

        private const string StartupFile = @"D:\Azari, Fritz Helder, Iii, Tesla Boy - PARAFFIN #emoll #mix@art-cafe.mp3";

        private void StartupMainView()
        {
            MainWindow = new HotMarkPlayerView(new PlayerViewModel(StartupFile));
            MainWindow.Show();
        }
    }
}