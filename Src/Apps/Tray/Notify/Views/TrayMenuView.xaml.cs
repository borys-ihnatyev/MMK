using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MMK.Presentation.Windows;

namespace MMK.Notify.Views
{
    public partial class TrayMenuView
    {
        private readonly Storyboard hideStoryboard;
        private readonly Storyboard showStoryboard;

        public TrayMenuView()
        {
            InitializeComponent();
            
            showStoryboard = FindResource("ShowStoryboard") as Storyboard;
            showStoryboard.Completed += (s, e) => Activate();
            hideStoryboard = FindResource("HideStoryboard") as Storyboard;
            
            Loaded += OnLoaded;
            Deactivated += (s, e) => Hide();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetStartupPosition();
            Show();
        }

        private void SetStartupPosition()
        {
            var taskbar = new Taskbar();

            switch (taskbar.Position)
            {
                case Taskbar.TaskbarPosition.Left:
                    Top = SystemParameters.WorkArea.Bottom - Height - 7;
                    Left = SystemParameters.WorkArea.Left + Width + 7;
                    break;

                case Taskbar.TaskbarPosition.Top:
                    Top = SystemParameters.WorkArea.Top + 7;
                    Left = SystemParameters.WorkArea.Right - Width - 7;
                    break;

                case Taskbar.TaskbarPosition.Right:
                    Top = SystemParameters.WorkArea.Bottom - Height - 7;
                    Left = SystemParameters.WorkArea.Right - Width - 7;
                    break;

                case Taskbar.TaskbarPosition.Bottom:
                    Top = SystemParameters.WorkArea.Bottom - Height - 7;
                    Left = SystemParameters.WorkArea.Right - Width - 7;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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

        private void CloseCommandAction(object sender, ExecutedRoutedEventArgs e)
        {
            hideStoryboard.Completed += (s,a) => Application.Current.Shutdown();
            Hide();
        }
    }
}