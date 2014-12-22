using System.Windows.Forms;
using System.Windows.Threading;
using MMK.Wpf.Windows;

namespace MMK.Notify.Views
{
    public partial class TaskProgressView
    {
        private readonly Taskbar taskbar;

        public TaskProgressView()
        {
            InitializeComponent();
            Opacity = 0;
            taskbar = new Taskbar();
        }

        public new void Show()
        {
            SetStartupPosition();
            Opacity = 1;
            base.Show();
            
        }

        public new void Hide()
        {
            base.Hide();
            Opacity = 0;
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