using System.ComponentModel;
using System.Windows;

namespace MMK.HotMark.PianoKeyboardUsage
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private MainViewModel ViewModel
        {
            get { return DataContext as MainViewModel; }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.LoadData();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ViewModel.UnloadData();
            base.OnClosing(e);
        }
    }
}
