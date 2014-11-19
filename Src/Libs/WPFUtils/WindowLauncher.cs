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
            Window = OnCreateWindow();

            BindWindowEvents();
        }

        protected virtual TWindow OnCreateWindow()
        {
            return new TWindow();
        }

        private void BindWindowEvents()
        {
            Window.Closed += (sender, args) => Window = null;

            OnBindWindowEvents();
        }

        protected virtual void OnBindWindowEvents() { }

        protected virtual void BeforeLaunch() { }
    }
}
