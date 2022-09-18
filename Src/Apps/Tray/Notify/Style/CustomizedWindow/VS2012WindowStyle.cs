using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace MMK.Notify.Style.CustomizedWindow
{
    public partial class Vs2012WindowStyle
    {
        #region sizing event handlers

        private void OnSizeSouth(object sender, MouseButtonEventArgs e)
        {
            OnSize(sender, SizingAction.South);
        }

        private void OnSizeNorth(object sender, MouseButtonEventArgs e)
        {
            OnSize(sender, SizingAction.North);
        }

        private void OnSizeEast(object sender, MouseButtonEventArgs e)
        {
            OnSize(sender, SizingAction.East);
        }

        private void OnSizeWest(object sender, MouseButtonEventArgs e)
        {
            OnSize(sender, SizingAction.West);
        }

        private void OnSizeNorthWest(object sender, MouseButtonEventArgs e)
        {
            OnSize(sender, SizingAction.NorthWest);
        }

        private void OnSizeNorthEast(object sender, MouseButtonEventArgs e)
        {
            OnSize(sender, SizingAction.NorthEast);
        }

        private void OnSizeSouthEast(object sender, MouseButtonEventArgs e)
        {
            OnSize(sender, SizingAction.SouthEast);
        }

        private void OnSizeSouthWest(object sender, MouseButtonEventArgs e)
        {
            OnSize(sender, SizingAction.SouthWest);
        }

        private void OnSize(object sender, SizingAction action)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                sender.ForWindowFromTemplate(w =>
                {
                    if (w.WindowState == WindowState.Normal)
                        DragSize(w.GetWindowHandle(), action);
                });
            }
        }

        private void IconMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                sender.ForWindowFromTemplate(w => w.Close());
            }
            else
            {
                sender.ForWindowFromTemplate(w =>
                    SendMessage(w.GetWindowHandle(), WmSyscommand, (IntPtr) ScKeymenu, (IntPtr) ' '));
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            sender.ForWindowFromTemplate(w => w.Close());
        }

        private void MinButtonClick(object sender, RoutedEventArgs e)
        {
            sender.ForWindowFromTemplate(w => w.WindowState = WindowState.Minimized);
        }

        private void MaxButtonClick(object sender, RoutedEventArgs e)
        {
            sender.ForWindowFromTemplate(
                w =>
                    w.WindowState =
                        (w.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized);
        }

        private void TitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                MaxButtonClick(sender, e);
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                sender.ForWindowFromTemplate(w => w.DragMove());
            }
        }

        private void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                sender.ForWindowFromTemplate(w =>
                {
                    if (w.WindowState == WindowState.Maximized)
                    {
                        w.BeginInit();
                        const double adjustment = 40.0;
                        var mouse1 = e.MouseDevice.GetPosition(w);
                        var width1 = Math.Max(w.ActualWidth - 2*adjustment, adjustment);
                        w.WindowState = WindowState.Normal;
                        var width2 = Math.Max(w.ActualWidth - 2*adjustment, adjustment);
                        w.Left = (mouse1.X - adjustment)*(1 - width2/width1);
                        w.Top = -7;
                        w.EndInit();
                        w.DragMove();
                    }
                });
            }
        }

        #endregion

        #region P/Invoke

        private const int WmSyscommand = 0x112;
        private const int ScSize = 0xF000;
        private const int ScKeymenu = 0xF100;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private static void DragSize(IntPtr handle, SizingAction sizingAction)
        {
            SendMessage(handle, WmSyscommand, (IntPtr) (ScSize + sizingAction), IntPtr.Zero);
            SendMessage(handle, 514, IntPtr.Zero, IntPtr.Zero);
        }

        public enum SizingAction
        {
            North = 3,
            South = 6,
            East = 2,
            West = 1,
            NorthEast = 5,
            NorthWest = 4,
            SouthEast = 8,
            SouthWest = 7
        }

        #endregion
    }
}