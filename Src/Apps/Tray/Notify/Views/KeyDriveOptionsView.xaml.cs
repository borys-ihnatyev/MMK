using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MMK.Notify.ViewModels;
using MMK.Presentation.Extensions;

namespace MMK.Notify.Views
{
    public partial class KeyDriveOptionsView
    {
        public KeyDriveOptionsView(KeyDriveOptionsViewModel keyDriveOptionsViewModel)
        {
            DataContext = keyDriveOptionsViewModel;
            Loaded += (s, e) => keyDriveOptionsViewModel.LoadData();
            Loaded += OnLoaded;
            Closed += (s, e) => keyDriveOptionsViewModel.UnloadData();
            
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs ea)
        {
            this.FindVisualChildren<Button>().ForEach(b => b.Click += (s, e) => Close());
        }
    }
}