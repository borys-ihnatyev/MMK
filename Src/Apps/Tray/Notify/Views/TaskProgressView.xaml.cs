using System.Windows.Forms;
using System.Windows.Input;
using MMK.Wpf.Windows;

namespace MMK.Notify.Views
{
    public partial class TaskProgressView
    {
        private readonly Taskbar taskbar;

        public TaskProgressView()
        {
            Opacity = 0;
            taskbar = new Taskbar();
            MouseDown += OnMouseDown;
            Loaded += (s, e) => SetStartupPosition();
            InitializeComponent();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void SetStartupPosition()
        {
            switch (taskbar.Position)
            {
                case Taskbar.TaskbarPosition.Bottom:
                case Taskbar.TaskbarPosition.Right:
                    Top = Screen.PrimaryScreen.WorkingArea.Bottom - Height - 7;
                    Left = Screen.PrimaryScreen.WorkingArea.Right - Width - 7;
                    break;
                case Taskbar.TaskbarPosition.Left:
                    Top = Screen.PrimaryScreen.WorkingArea.Bottom - Height - 7;
                    Left = Screen.PrimaryScreen.WorkingArea.Left + 7;
                    break;
                case Taskbar.TaskbarPosition.Top:
                    Top = Screen.PrimaryScreen.WorkingArea.Top + 7;
                    Left = Screen.PrimaryScreen.WorkingArea.Right - Width - 7;
                    break;
            }
        }
    }
}