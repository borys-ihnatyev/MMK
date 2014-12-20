using System;
using System.Windows;
using System.Windows.Forms;
using MMK.Notify.ViewModel.TrayMenu;
using MMK.Wpf.Windows;

namespace MMK.Notify.View.TrayMenu
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
            SetStartupPosition();
        }

        private void SetStartupPosition()
        {
            var taskbar = new Taskbar();

            switch (taskbar.Position)
            {
                case Taskbar.TaskbarPosition.Left:
                    Top = Screen.PrimaryScreen.WorkingArea.Bottom - Height - 7;
                    Left = Screen.PrimaryScreen.WorkingArea.Left + Width + 7;
                    break;

                case Taskbar.TaskbarPosition.Top:
                    Top = Screen.PrimaryScreen.WorkingArea.Top + 7;
                    Left = Screen.PrimaryScreen.WorkingArea.Right - Width - 7;
                    break;

                case Taskbar.TaskbarPosition.Right:
                    Top = Screen.PrimaryScreen.WorkingArea.Bottom - Height - 7;
                    Left = Screen.PrimaryScreen.WorkingArea.Right - Width - 7;
                    break;

                case Taskbar.TaskbarPosition.Bottom:
                    Top = Screen.PrimaryScreen.WorkingArea.Bottom - Height - 7;
                    Left = Screen.PrimaryScreen.WorkingArea.Right - Width - 7;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}