using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MMK.HotMark.ViewModels;

namespace MMK.HotMark.Views
{
    public partial class HotMarkMainView
    {
        public HotMarkMainView(HotMarkViewModel viewModel)
        {
            InitializeComponent();
            
            DataContext = viewModel;

            Loaded += (s, e) => viewModel.LoadData();
            Loaded += Window_Loaded;
            SizeChanged += OnSizeChanged;
            MouseDown += OnMouseDown;

            Closing += (s, e) => viewModel.UnloadData();

            isCloseStoryboardStarted = false;
            isCloseStoryboardCompletted = false;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                SizeChanged -= OnSizeChanged;
                DragMove();
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(!e.WidthChanged)
                return;
            Left = (Screen.PrimaryScreen.WorkingArea.Width - Width)/2;
        }

        #region Animation Flags

        private bool isCloseStoryboardStarted;
        private bool isCloseStoryboardCompletted;

        #endregion

        #region Event Handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Focus();
            Activate();
        }

        private void CloseStoryboard_Completed(object sender, EventArgs e)
        {
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
                typeof (HotMarkMainView)
                );

        public event RoutedEventHandler UserClose
        {
            add { AddHandler(UserCloseEvent, value); }
            remove { RemoveHandler(UserCloseEvent, value); }
        }

        #endregion
    }
}