namespace MMK.MusicCollectionManager
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
        }
    }
}
