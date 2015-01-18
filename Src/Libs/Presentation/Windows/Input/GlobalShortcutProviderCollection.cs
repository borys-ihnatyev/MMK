using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Input;
using MMK.Presentation.Windows.Interop;

namespace MMK.Presentation.Windows.Input
{
    public class GlobalShortcutProviderCollection : IGlobalShortcutProvider, ICollection<IGlobalShortcutProvider>
    {
        private readonly IHwndSource hwndSource;

        private readonly HashSet<IGlobalShortcutProvider> shortcutProviders;

        public GlobalShortcutProviderCollection(IHwndSource hwndSource)
        {
            if(hwndSource == null)
                throw new ArgumentNullException("hwndSource");
            Contract.EndContractBlock();

            this.hwndSource = hwndSource;
            shortcutProviders = new HashSet<IGlobalShortcutProvider>();
        }


        public int Count
        {
            get { return shortcutProviders.Count; }
        }

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
            shortcutProviders.Add(shortcutProvider);
        }

        public IGlobalShortcutProvider Add(ModifierKeys modifier, System.Windows.Input.Key key, Action pressed)
        {
            var shortcutProvider = new GlobalShortcutProvider(hwndSource, modifier, key);
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