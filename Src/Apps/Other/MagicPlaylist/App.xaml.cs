using System.Windows;
using MMK.MagicPlaylist.ViewModels;

namespace MMK.MagicPlaylist
{
    public partial class App
    {
        private static MainViewModel mainViewModel;

        public static MainViewModel MainViewModel
        {
            get
            {
                mainViewModel = mainViewModel ?? new MainViewModel();
                return mainViewModel;
            }
        }
    }
}
