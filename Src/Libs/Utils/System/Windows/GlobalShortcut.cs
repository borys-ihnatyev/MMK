using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Windows.Input;

// ReSharper disable once CheckNamespace

namespace System.Windows
{
    public class GlobalShortcut
    {
        public const int WmHotkey = 0x0312;

        private readonly IntPtr hwnd;
        protected readonly ModifierKeys Modifiers;
        protected readonly int KeyCode;

        public GlobalShortcut(ModifierKeys modifierKeys, Key key, IntPtr hwnd)
            : this(modifierKeys, KeyInterop.VirtualKeyFromKey(key), hwnd)
        {
        }

        public GlobalShortcut(ModifierKeys modifierKeys, Key key)
            : this(modifierKeys, KeyInterop.VirtualKeyFromKey(key), IntPtr.Zero)
        {
        }

        public GlobalShortcut(ModifierKeys modifiers, int keyCode, IntPtr hwnd)
        {
            Modifiers = modifiers;
            KeyCode = keyCode;
            this.hwnd = hwnd;
        }

        public GlobalShortcut(GlobalShortcut shortcut, IntPtr hwnd)
        {
            Modifiers = shortcut.Modifiers;
            KeyCode = shortcut.KeyCode;
            this.hwnd = hwnd;
        }

        public int Id
        {
            get { return GetHashCode(); }
        }

        public bool IsRegistred { get; private set; }

        public bool Register()
        {
            if (IsRegistred) return IsRegistred;

            IsRegistred = User32.RegisterHotKey(hwnd, Id, Modifiers, KeyCode);

            if (IsRegistred) return IsRegistred;

            User32.UnregisterHotKey(hwnd, Id);
            IsRegistred = User32.RegisterHotKey(hwnd, Id, Modifiers, KeyCode);
            return IsRegistred;
        }

        public bool Unregister()
        {
            if (IsRegistred)
                IsRegistred = !User32.UnregisterHotKey(hwnd, Id);
            return !IsRegistred;
        }

        protected bool Equals(GlobalShortcut other)
        {
            Contract.Requires(other != null);
            Contract.EndContractBlock();

            return hwnd.Equals(other.hwnd) && Modifiers == other.Modifiers && KeyCode == other.KeyCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GlobalShortcut) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = hwnd.GetHashCode();
                hashCode = (hashCode*397) ^ (int) Modifiers;
                hashCode = (hashCode*397) ^ KeyCode;
                return hashCode;
            }
        }

        private static class User32
        {
            [DllImport("user32.dll")]
            public static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys modifiers, int keyCode);

            [DllImport("user32.dll")]
            public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        }
    }
}