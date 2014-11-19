using System.Diagnostics.Contracts;
using System.Windows.Forms;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace System.Windows
{
    [Serializable]
    [Flags]
    public enum KeyModifyers
    {
        None = 0,
        Alt = 1,
        Ctrl = 2,
        Shift = 4,
        Win = 8,
        Norep = 0x8000
    }

    public class GlobalShortcut 
    {
        public const int WM_HOTKEY = 0x0312;

        private readonly IntPtr hwnd;
        protected readonly KeyModifyers Modifyers;
        protected readonly int KeyCode;

        public GlobalShortcut(KeyModifyers modifyers, int keyCode, IntPtr hwnd)
            : this()
        {
            Modifyers = modifyers;
            KeyCode = keyCode;
            this.hwnd = hwnd;
        }

        public GlobalShortcut(KeyModifyers modifyers, int keyCode) 
            : this(modifyers, keyCode, IntPtr.Zero)
        {
        }

        public GlobalShortcut(KeyModifyers modifyers, Keys keyCode, IntPtr hwnd)
            : this(modifyers, (int)keyCode, hwnd)
        {
        }

        public GlobalShortcut(GlobalShortcut shortcut, IntPtr hwnd)
        {
            Modifyers = shortcut.Modifyers;
            KeyCode = shortcut.KeyCode;
            this.hwnd = hwnd;
        }

        protected GlobalShortcut()
        {
            IsRegistred = false;
        }

        public int Id
        {
            get { return GetHashCode(); }
        }

        public bool IsRegistred { get; private set; }

        public bool Register()
        {
            if (IsRegistred) return IsRegistred;
            
            IsRegistred = User32.RegisterHotKey(hwnd, Id, Modifyers, KeyCode);

            if (IsRegistred) return IsRegistred;
    
            User32.UnregisterHotKey(hwnd, Id);
            IsRegistred = User32.RegisterHotKey(hwnd, Id, Modifyers, KeyCode);
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

            return hwnd.Equals(other.hwnd) && Modifyers == other.Modifyers && KeyCode == other.KeyCode;
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
                hashCode = (hashCode*397) ^ (int) Modifyers;
                hashCode = (hashCode*397) ^ KeyCode;
                return hashCode;
            }
        }

        private static class User32
        {
            [DllImport("user32.dll")]
            public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifyers modifiers, int keyCode);

            [DllImport("user32.dll")]
            public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        }
    }
}