using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MMK.Wpf.Providers
{
    public class GlobalShortcutProviderCollection : IGlobalShortcutProvider, ICollection<IGlobalShortcutProvider>
    {
        private Window window;

        private readonly HashSet<IGlobalShortcutProvider> hotkeyProviders;

        public GlobalShortcutProviderCollection()
        {
            hotkeyProviders = new HashSet<IGlobalShortcutProvider>();
        }

        public GlobalShortcutProviderCollection(Window window):this()
        {
            this.window = window;
        }

        public void StartListen()
        {
            if(IsListening) return;
            hotkeyProviders.ForEach(p => p.StartListen());            
        }

        public void StopListen()
        {
            if(!IsListening) return;
            hotkeyProviders.ForEach(p => p.StopListen());
        }

        public bool Add(IGlobalShortcutProvider provider)
        {
            provider.SetWindow(window);
            return hotkeyProviders.Add(provider);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IGlobalShortcutProvider item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IGlobalShortcutProvider[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IGlobalShortcutProvider item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; }

        public bool Add(KeyModifyers modifyer, int keyCode, Action pressed)
        {
            var provider = new GlobalShortcutProvider(window, modifyer, keyCode);
            provider.Pressed += pressed;
            return Add(provider);
        }


        public bool IsListening
        {
            get { return hotkeyProviders.Any(p => p.IsListening); }
        }

        void IGlobalShortcutProvider.SetWindow(Window newWindow)
        {
            var startListen = false;
            
            if (IsListening)
            {
                StopListen();
                startListen = true;
            }

            hotkeyProviders.ForEach(p => p.SetWindow(newWindow));

            if(startListen)
                StartListen();

            window = newWindow;
        }

        void IGlobalShortcutProvider.StartListen()
        {
            StartListen();
        }

        void IGlobalShortcutProvider.StopListen()
        {
            StopListen();
        }

        public IEnumerator<IGlobalShortcutProvider> GetEnumerator()
        {
            return hotkeyProviders.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<IGlobalShortcutProvider>.Add(IGlobalShortcutProvider item)
        {
            throw new NotImplementedException();
        }
    }
}
