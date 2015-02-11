using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MMK.HotMark.ViewModels;

namespace MMK.HotMark.Views
{
    public partial class HotMarkMainView
    {
        private readonly Action applyChangesAction; 

        private readonly Storyboard showStoryboard;
        private readonly Storyboard hideStoryboard;

        public HotMarkMainView(HotMarkViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            (viewModel.HashTags as INotifyPropertyChanged).PropertyChanged += OnHashTagsPropertyChanged;
            Loaded += (s, e) => viewModel.LoadData();
            Closed += (s, e) => viewModel.UnloadData();

            applyChangesAction = viewModel.NotifyChange;

            showStoryboard = FindResource("ShowStoryboard") as Storyboard;
            Contract.Assume(showStoryboard != null);
            showStoryboard.Completed += (s, e) => Activate();
            hideStoryboard = FindResource("HideStoryboard") as Storyboard;

            SizeChanged += OnSizeChanged;
            MouseDown += OnMouseDown;
        }

        #region Event handlers

        private void OnHashTagsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                Keyboard.Focus(HashTagEditBox); 
                HashTagEditBox.TextChanged += HashTagEditBoxPutCarretAtEndOnce;
            }
        }

        private void HashTagEditBoxPutCarretAtEndOnce(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            HashTagEditBox.CaretIndex = Int32.MaxValue;
            HashTagEditBox.TextChanged -= HashTagEditBoxPutCarretAtEndOnce;
        }
 

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
                return;
            Left = SystemParameters.WorkArea.Left + (SystemParameters.WorkArea.Width - e.NewSize.Width)/2;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(ReferenceEquals(sender, this))
                if (e.ChangedButton == MouseButton.Left)
                {
                    SizeChanged -= OnSizeChanged;
                    DragMove();
                }
        }

        #endregion

        public new void Show()
        {
            hideStoryboard.Stop();
            showStoryboard.Begin(this);
        }

        public new void Hide()
        {
            showStoryboard.Stop();
            hideStoryboard.Begin(this);
        }

        private void OnCloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var applyChanges = (bool)e.Parameter;
            
            if(applyChanges)
                Closed += (s, a) => applyChangesAction();

            hideStoryboard.Completed += (s, a) => Close();
            Hide();
        }
    }
}