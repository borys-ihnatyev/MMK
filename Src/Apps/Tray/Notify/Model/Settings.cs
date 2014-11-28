using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MMK.Processing.AutoFolder;

namespace MMK.Notify.Model
{
    [Serializable]
    public sealed class Settings
    {
        public Settings()
        {
            IsStartMusicDownloadWatching = false;
            IsEnableHotKeysMenuItem = false;
            FolderCollection = DefaultFolderCollection();
        }

        #region Singletone

        [NonSerialized] private static Settings instance;

        public static Settings Load()
        {
            return instance ?? (instance = TryDeserialize());
        }

        private static Settings TryDeserialize()
        {
            if (!File.Exists(DefaultSettingsFileName))
                return new Settings();

            using (var stream = File.Open(DefaultSettingsFileName, FileMode.Open))
            {
                try
                {
                    var serizlizer = new BinaryFormatter();
                    return (Settings) serizlizer.Deserialize(stream);
                }
                catch (SerializationException)
                {
                    return new Settings();
                }
            }
        }

        #endregion

        [NonSerialized] private const string DefaultSettingsFileName = "MMK.Notify.Settings.bin";

        public bool IsStartMusicDownloadWatching { get; set; }

        public bool IsEnableHotKeysMenuItem { get; set; }

        public HashTagFolderCollection FolderCollection { get; set; }

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

        public void Save()
        {
            using (var stream = File.Create(DefaultSettingsFileName))
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(stream, this);
            }
        }
    }
}