using MMK.Utils;

namespace MMK.Notify.Model.Launchers
{
    sealed public class MusicCollectionManagerLauncher : SingleAppLauncher
    {
        private static MusicCollectionManagerLauncher instance;

        public static MusicCollectionManagerLauncher Instance
        {
            get { return instance ?? (instance = new MusicCollectionManagerLauncher()); }
        }

        public override string Path
        {
            get { return "MMK.MusicCollectionManager.exe"; }
        }
    }
}
