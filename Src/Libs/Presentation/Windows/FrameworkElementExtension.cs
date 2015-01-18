using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows;

namespace MMK.Presentation.Windows
{
    public static class FrameworkElementExtension
    {
        public static void AfterLoad(this FrameworkElement element, Action handler)
        {
            if(handler == null)
                throw new ArgumentNullException("handler");
            Contract.EndContractBlock();

            if (element.IsLoaded)
                handler();
            else
                element.Loaded += (s, e) => handler();
        }

        public static void AfterLoad(this FrameworkElement element, params Action[] handlers)
        {
            if(!Contract.ForAll(handlers, h => h != null))
                throw new ArgumentNullException("handlers");
            Contract.EndContractBlock();

            if (element.IsLoaded)
                handlers.ForEach(h => h.Invoke());
            else
                element.Loaded += (s, e) => handlers.ForEach(h => h.Invoke());
        }
    }
}