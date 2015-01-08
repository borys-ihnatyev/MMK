using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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
            (viewModel.HashTags as INotifyPropertyChanged).PropertyChanged += OnHashTagsPropertyChanged;
            Loaded += (s, e) => viewModel.LoadData();
            Loaded += OnLoaded;

            SizeChanged += OnSizeChanged;
            MouseDown += OnMouseDown;

            Closing += (s, e) => viewModel.UnloadData();

            isCloseStoryboardStarted = false;
            isCloseStoryboardCompletted = false;
        }

        private void OnHashTagsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
                HashTagEditBoxFocus();
        }

        private void HashTagEditBoxFocus()
        {
            FocusManager.SetFocusedElement(this, HashTagEditBox);
            HashTagEditBox.TextChanged += HashTagEditBoxPutCarretAtEndOnce;
        }

        private void HashTagEditBoxPutCarretAtEndOnce(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            HashTagEditBox.CaretIndex = Int32.MaxValue;
            HashTagEditBox.TextChanged -= HashTagEditBoxPutCarretAtEndOnce;
        }

        #region Animation Flags

        private bool isCloseStoryboardStarted;

        private bool isCloseStoryboardCompletted;

        #endregion

        #region Event Handlers

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Focus();
            Activate();
            HashTagEditBoxFocus();
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
            if (!e.WidthChanged)
                return;
            Left = SystemParameters.WorkArea.Left + (SystemParameters.WorkArea.Width - e.NewSize.Width)/2;
        }

        private void OnCloseStoryboardCompleted(object sender, EventArgs e)
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