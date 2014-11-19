using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using MMK.HotMark.ViewModel.Main;

namespace MMK.HotMark.View
{
    public partial class MainView
    {
        private readonly MainViewModel viewModel;

        public MainView() : this(new string[0])
        {
            
        }

        public MainView(IEnumerable<string> paths)
        {
            InitializeComponent();
            
            viewModel = new MainViewModel(paths);
            DataContext = viewModel;
            
            isCloseStoryboardStarted = false;
            isCloseStoryboardCompletted = false;
            
            Loaded += Window_Loaded;
        }

        #region Animation Flags

        private bool isCloseStoryboardStarted;
        private bool isCloseStoryboardCompletted;

        #endregion

        #region Event Handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.LoadData();
            Focus();
        }

        private void CloseStoryboard_Completed(object sender, EventArgs e)
        {
            viewModel.UnloadData();
            isCloseStoryboardCompletted = true;
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!isCloseStoryboardStarted)
            {
                isCloseStoryboardStarted = true;
                RaiseEvent(new RoutedEventArgs(UserCloseEvent));
                e.Cancel = true;
            }
            else if (!isCloseStoryboardCompletted)
                e.Cancel = true;

            base.OnClosing(e);
        }

        #endregion

        #region Events

        public static readonly RoutedEvent UserCloseEvent =
            EventManager.RegisterRoutedEvent(
                "UserClose",
                RoutingStrategy.Direct,
                typeof (RoutedEventHandler),
                typeof (MainView)
                );

        public event RoutedEventHandler UserClose
        {
            add { AddHandler(UserCloseEvent, value); }
            remove { RemoveHandler(UserCloseEvent, value); }
        }

        #endregion
    }
}