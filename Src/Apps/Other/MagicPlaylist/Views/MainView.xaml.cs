namespace MMK.MagicPlaylist.Views
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = App.MainViewModel;
        }
    }
}
