using System;
using System.Diagnostics.Contracts;
using System.Windows;

namespace MMK.Wpf
{
    public class WindowLauncher<TWindow> where TWindow : Window
    {
        private readonly Func<TWindow> factoryMethod;

        public WindowLauncher(Func<TWindow> factoryMethod)
        {
            if(factoryMethod == null)
                throw new ArgumentNullException("factoryMethod");
            Contract.EndContractBlock();

            this.factoryMethod = factoryMethod;
        }

        public WindowLauncher()
        {
            
        }

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
            Window = factoryMethod();

            BindWindowEvents();
        }

        protected virtual TWindow WindowFactory()
        {
            if(factoryMethod == null)
                throw new InvalidOperationException("if no factory method passed in ctor, then WindowFactory must be overriden");
            Contract.EndContractBlock();

            return factoryMethod();
        }

        protected virtual void BindWindowEvents()
        {
            Window.Closed += (sender, args) => Window = null;
        }

        protected virtual void BeforeLaunch() { }
    }
}
