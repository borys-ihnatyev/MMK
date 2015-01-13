using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using MMK.Wpf.Windows;

namespace MMK.Notify.Views
{
    public partial class TrayMenuView
    {
        private readonly Storyboard showStoryboard;
        private readonly Storyboard hideStoryboard;

        public TrayMenuView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            showStoryboard = FindResource("ShowStoryboard") as Storyboard;
            hideStoryboard = FindResource("HideStoryboard") as Storyboard;
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
            showStoryboard.Begin(this);
        }

        public new void Hide()
        {
            hideStoryboard.Begin(this);
        }
    }
}