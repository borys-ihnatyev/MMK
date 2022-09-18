using System.Windows.Input;
using MMK.HotMark.ViewModels;

namespace MMK.HotMark.MediaPlayer.Views
{
    public partial class HotMarkPlayerView
    {
        public HotMarkPlayerView(PlayerViewModel hotMarkPlayerViewModel)
        {
            DataContext = hotMarkPlayerViewModel;
            Closing += (s, e) => hotMarkPlayerViewModel.UnloadData();
            MouseDown += OnMouseDown;
            InitializeComponent();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
    }
}