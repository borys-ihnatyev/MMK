using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MMK.Wpf.Providers
{
    public class GlobalShortcutProviderCollection : IGlobalShortcutProvider, ICollection<IGlobalShortcutProvider>
    {
        private Window ownerWindow;

        private readonly HashSet<IGlobalShortcutProvider> shortcutProviders;

        public GlobalShortcutProviderCollection() : this(null)
        {
        }

        public GlobalShortcutProviderCollection(Window ownerWindow)
        {
            shortcutProviders = new HashSet<IGlobalShortcutProvider>();
            this.ownerWindow = ownerWindow;
        }


        public int Count { get { return shortcutProviders.Count; } }

        public bool IsAnyListening
        {
            get { return shortcutProviders.Any(p => p.IsListening); }
        }


        public void StartListen()
        {
            shortcutProviders.ForEach(p => p.StartListen());
        }

        public void StopListen()
        {
            shortcutProviders.ForEach(p => p.StopListen());
        }


        public void Add(IGlobalShortcutProvider shortcutProvider)
        {
            shortcutProvider.SetWindow(ownerWindow);
            shortcutProviders.Add(shortcutProvider);
        }

        public IGlobalShortcutProvider Add(KeyModifyers modifyer, int keyCode, Action pressed)
        {
            var shortcutProvider = new GlobalShortcutProvider(ownerWindow, modifyer, keyCode);
            shortcutProvider.Pressed += pressed;
            Add(shortcutProvider);
            return shortcutProvider;
        }

        public bool Contains(IGlobalShortcutProvider shortcutProvider)
        {
            return shortcutProviders.Contains(shortcutProvider);
        }

        public bool Remove(IGlobalShortcutProvider shortcutProvider)
        {
            return shortcutProviders.Remove(shortcutProvider);
        }

        public void Clear()
        {
            shortcutProviders.Clear();
        }


        public IEnumerator<IGlobalShortcutProvider> GetEnumerator()
        {
            return shortcutProviders.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IGlobalShortcutProvider.SetWindow(Window newWindow)
        {
            var startListen = false;
            
            if (IsAnyListening)
            {
                StopListen();
                startListen = true;
            }

            shortcutProviders.ForEach(p => p.SetWindow(newWindow));

            if(startListen)
                StartListen();

            ownerWindow = newWindow;
        }

        bool IGlobalShortcutProvider.IsListening 
        {
            get { return IsAnyListening; }
        }

        bool ICollection<IGlobalShortcutProvider>.IsReadOnly
        {
            get { return false; }
        }

        void ICollection<IGlobalShortcutProvider>.CopyTo(IGlobalShortcutProvider[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }
    }
}
