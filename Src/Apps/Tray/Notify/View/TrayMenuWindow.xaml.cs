using System.Windows;
using System.Windows.Forms;
using MMK.Notify.ViewModel.TrayMenu;

namespace MMK.Notify.View
{
    public partial class TrayMenuWindow
    {
        public TrayMenuWindow(TrayMenuViewModel viewModel)
        {
            DataContext = viewModel;

            Loaded += OnLoaded;
 
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Top = Screen.PrimaryScreen.WorkingArea.Bottom - Height - 7;
            Left = Screen.PrimaryScreen.WorkingArea.Right - Width - 7;
        }
    }
}
