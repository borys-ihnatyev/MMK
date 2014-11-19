using MMK.MusicCollectionManager.ViewModel;

namespace MMK.MusicCollectionManager
{
    public partial class App
    {
        static MainViewModel viewModel;

        public static MainViewModel ViewModel
        {
            get { return viewModel ?? (viewModel = new MainViewModel()); }
        }
    }
}
