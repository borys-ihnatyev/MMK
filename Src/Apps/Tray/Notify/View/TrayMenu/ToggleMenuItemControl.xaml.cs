using System;
using System.Windows;
using System.Windows.Input;
using MMK.Wpf;

namespace MMK.Notify.View.TrayMenu
{
    public partial class ToggleMenuItemControl
    {
        public ToggleMenuItemControl()
        {
            InitializeComponent();
            Command = new Command(CommandAction);
        }

        public ICommand PressCommand
        {
            get { return (ICommand) GetValue(PressCommandProperty); }
            set { SetValue(PressCommandProperty, value); }
        }

        public ICommand ReleaseCommand
        {
            get { return (ICommand) GetValue(ReleaseCommandProperty); }
            set { SetValue(ReleaseCommandProperty, value); }
        }

        public bool IsPressed
        {
            get { return (bool) GetValue(IsPressedCommandProperty); }
            set { SetValue(IsPressedCommandProperty, value); }
        }

        private void CommandAction()
        {
            if (IsPressed)
            {
                if (ReleaseCommand != null)
                    ReleaseCommand.Execute(null);
                
            }
            else
            {
                if (PressCommand != null)
                    PressCommand.Execute(null);
            }

            IsPressed = !IsPressed;
        }

        public static readonly DependencyProperty IsPressedCommandProperty =
            DependencyProperty.Register("IsPressed", typeof(Boolean), typeof(ToggleMenuItemControl),
                new PropertyMetadata(default(Boolean)));

        public static readonly DependencyProperty PressCommandProperty =
            DependencyProperty.Register("PressCommand", typeof (ICommand), typeof (ToggleMenuItemControl),
                new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty ReleaseCommandProperty =
            DependencyProperty.Register("ReleaseCommand", typeof (ICommand), typeof (ToggleMenuItemControl),
                new PropertyMetadata(default(ICommand)));
    }
}