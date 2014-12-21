using System.Windows;

namespace MMK.Wpf
{
    public class WindowLauncher<TWindow> where TWindow : Window, new()
    {
        public TWindow Window
        {
            get; private set;
        }

        public void Launch()
        {
            if (Window == null)
                CreateWindow();

            LaunchWindow();
        }

        private void LaunchWindow()
        {
            BeforeLaunch();
            Window.Show();
            Window.Activate();
        }

        private void CreateWindow()
        {
            Window = WindowFactory();

            BindWindowEvents();
        }

        protected virtual TWindow WindowFactory()
        {
            return new TWindow();
        }

        protected virtual void BindWindowEvents()
        {
            Window.Closed += (sender, args) => Window = null;
        }

        protected virtual void BeforeLaunch() { }
    }
}
