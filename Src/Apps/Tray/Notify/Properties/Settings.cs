using System.ComponentModel;
using System.Configuration;
using System.IO;
using MMK.Processing.AutoFolder;

namespace MMK.Notify.Properties
{
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings
    {
        public Settings()
        {
            SettingsLoaded += OnLoaded;
            PropertyChanged += OnChanged;
        }

        private void OnChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Save();
        }

        private void OnLoaded(object sender, SettingsLoadedEventArgs settingsLoadedEventArgs)
        {
            if (FolderCollection == null)
                FolderCollection = DefaultFolderCollection();
        }

        private static HashTagFolderCollection DefaultFolderCollection()
        {
            var popFolderPath = PathExtension.Normalize(@"D:\music\pop\");
            var danceFolderPath = PathExtension.Normalize(@"D:\music\dance\");
            var houseFolderPath = PathExtension.Normalize(@"D:\music\house\");
            var electronicFolderPath = PathExtension.Normalize(@"D:\music\electronic\");
            var urbanFolderPath = PathExtension.Normalize(@"D:\music\urban\");
            var deepFolderPath = PathExtension.Normalize(@"D:\music\deep\");
            var specFolderPath = PathExtension.Normalize(@"D:\music\spec\");
            var mixesFolderPath = PathExtension.Normalize(@"D:\music\mixes\");

            return new HashTagFolderCollection
            {
                {mixesFolderPath, "#mix", 100},
                {mixesFolderPath, "#mixes", 100},
                {deepFolderPath, "#deep", 3},
                {deepFolderPath, "#newdisco", 3},
                {urbanFolderPath, "#urban", 3},
                {urbanFolderPath, "#trap", 3},
                {houseFolderPath, "#house", 1},
                {houseFolderPath, "#electro", 1},
                {electronicFolderPath, "#electronic", 4},
                {danceFolderPath, "#dance", 1},
                {popFolderPath, "#pop"},
                {popFolderPath, "#rus"},
                {popFolderPath, "#ukr"},
                {specFolderPath, "#spec", 2},
                {specFolderPath, "#rnb", 2},
            };
        }
    }
}