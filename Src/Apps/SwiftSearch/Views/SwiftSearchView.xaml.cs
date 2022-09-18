using System;
using System.Windows;
using System.Windows.Input;
using MMK.SwiftSearch.ViewModels;

namespace MMK.SwiftSearch.Views
{
    public partial class SwiftSearchView
    {
        private readonly SwiftSearchViewModel viewModel;

        public SwiftSearchView(SwiftSearchViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            
            DataContext = viewModel;

            viewModel.SearchSelfChanged += OnSearchChangedByViewModel;
            Loaded += (s, e) => viewModel.LoadData();
            Loaded += (s, e) => SearchTextBox.Focus();
            Closing += (s, e) => viewModel.UnloadData();

            SizeChanged += OnSizeChanged;

            Top = SystemParameters.WorkArea.Top + SystemParameters.WorkArea.Height*0.2;
        }

        private void OnSearchChangedByViewModel(object sender, EventArgs eventArgs)
        {
            SearchTextBox.CaretIndex = Int32.MaxValue;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
                Left = SystemParameters.WorkArea.Left + (SystemParameters.WorkArea.Width - e.NewSize.Width)/2;
        }

        private void OnCloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var applySearch = (bool) e.Parameter;
            
            if(applySearch)
                viewModel.ApplySearch();
            
            Close();
        }
    }
}