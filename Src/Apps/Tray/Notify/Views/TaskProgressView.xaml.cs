using System.Windows;
using System.Windows.Input;
using MMK.Presentation.Windows;

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
                    Top = SystemParameters.WorkArea.Bottom - Height - 7;
                    Left = SystemParameters.WorkArea.Right - Width - 7;
                    break;
                case Taskbar.TaskbarPosition.Left:
                    Top = SystemParameters.WorkArea.Bottom - Height - 7;
                    Left = SystemParameters.WorkArea.Left + 7;
                    break;
                case Taskbar.TaskbarPosition.Top:
                    Top = SystemParameters.WorkArea.Top + 7;
                    Left = SystemParameters.WorkArea.Right - Width - 7;
                    break;
            }
        }
    }
}