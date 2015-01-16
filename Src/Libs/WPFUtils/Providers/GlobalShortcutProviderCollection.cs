using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;

namespace MMK.Wpf.Providers
{
    public class GlobalShortcutProviderCollection : IGlobalShortcutProvider, ICollection<IGlobalShortcutProvider>
    {
        private Window window;

        private readonly HashSet<IGlobalShortcutProvider> shortcutProviders;

        public GlobalShortcutProviderCollection() : this(null)
        {
        }

        public GlobalShortcutProviderCollection(Window window)
        {
            shortcutProviders = new HashSet<IGlobalShortcutProvider>();
            this.window = window;
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
            shortcutProvider.SetWindow(window);
            shortcutProviders.Add(shortcutProvider);
        }

        public IGlobalShortcutProvider Add(KeyModifyers modifyer, int keyCode, Action pressed)
        {
            var shortcutProvider = new GlobalShortcutProvider(window, modifyer, keyCode);
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

        void IGlobalShortcutProvider.SetWindow(Window ownerWindow)
        {
            if (ownerWindow == null)
                throw new ArgumentNullException("ownerWindow");
            Contract.EndContractBlock();

            window = ownerWindow;

            var startListen = false;
            
            if (IsAnyListening)
            {
                StopListen();
                startListen = true;
            }

            shortcutProviders.ForEach(p => p.SetWindow(window));

            if(startListen)
                StartListen();
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
