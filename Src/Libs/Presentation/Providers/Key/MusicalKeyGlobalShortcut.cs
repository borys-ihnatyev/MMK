using System;
using System.Windows;
using MMK.Marking;

namespace MMK.Presentation.Providers.Key
{
    class MusicalKeyGlobalShortcut : GlobalShortcut
    {
        public MusicalKeyGlobalShortcut(MMK.Key key)
            : this(key, IntPtr.Zero)
        {
        }

        public MusicalKeyGlobalShortcut(MMK.Key key, IntPtr hwnd)
            : base(ModifyersFrom(key), KeyCodeFrom(key), hwnd)
        {
            Key = key;
        }

        public MMK.Key Key { get; private set; }

        private static KeyModifyers ModifyersFrom(MMK.Key key)
        {
            var modifyers = key.IsMoll() ? KeyModifyers.Ctrl : KeyModifyers.Shift;

            if (key.IsSharpness())
                modifyers |= KeyModifyers.Alt;

            return modifyers;
        }

        private static int KeyCodeFrom(MMK.Key key)
        {
            return key.Note.ToString()[0];
        }

        public override int GetHashCode()
        {
            return (KeyCode << 4) | (int) Modifyers;
        }

        public static MMK.Key DecodeKey(int id)
        {
            var keyStr = string.Empty;
            keyStr += (char) (id >> 4);

            if ((id & (int) KeyModifyers.Alt) == (int) KeyModifyers.Alt)
                keyStr += '#';

            if ((id & (int) KeyModifyers.Ctrl) == (int) KeyModifyers.Ctrl)
                keyStr += 'm';

            return KeyHashTag.Parser.First("#" + keyStr).HashTag.Key;
        }
    }
}