using System.Windows;
using MMK.Notify.ViewModels.HashTagFolders;

namespace MMK.Notify.Views
{
    /// <summary>
    /// Interaction logic for HashTagFoldersWindow.xaml
    /// </summary>
    public partial class HashTagFoldersWindow
    {
        private readonly HashTagFoldersViewModel viewModel;

        public HashTagFoldersWindow()
        {
            InitializeComponent();
            viewModel = new HashTagFoldersViewModel();
            DataContext = viewModel;
            Loaded += WindowLoaded;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            viewModel.LoadData();
        }


    }
}
