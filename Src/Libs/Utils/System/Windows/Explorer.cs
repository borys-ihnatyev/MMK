using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using MMK.Utils;
using SHDocVw;

// ReSharper disable once CheckNamespace
namespace System.Windows
{
    public class Explorer
    {
        public static IEnumerable<string> GetForegroundSelectedItemsFileTree(string extension = "")
        {
            var extensionFilter = "*" + extension;

            var paths = GetForeGroundSelectedFilesAndDirs().ToList();

            var files = paths.Where(File.Exists);

            if (!String.IsNullOrWhiteSpace(extension))
                files = files.Where(file => FileExtensionParser.HasExtension(file, extension));

            var filesList = new LinkedList<string>(files);
            
            paths
                .Where(Directory.Exists)
                .SelectMany(d => Directory.EnumerateFiles(d, extensionFilter, SearchOption.AllDirectories))
                .ForEach(file => filesList.AddLast(file));

            return filesList;
        } 

        public static IEnumerable<string> GetForegroundSelectedItems()
        {
            var foregroundWindowHwnd = GetForegroundWindow();
            return GetSelectedItemsPaths(foregroundWindowHwnd);
        }

        public static IEnumerable<string> GetForeGroundSelectedFiles()
        {
            return GetForegroundSelectedItems().Where(File.Exists);
        }

        public static IEnumerable<string> GetForeGroundSelectedDirectories()
        {
            return GetForegroundSelectedItems().Where(Directory.Exists);
        }

        public static IEnumerable<string> GetForeGroundSelectedFilesAndDirs()
        {
            return GetForegroundSelectedItems().Where(p => File.Exists(p) || Directory.Exists(p));
        } 

        private static IEnumerable<string> GetSelectedItemsPaths(int hwnd)
        {
            foreach (var items in GetExplorerSelectedItems(hwnd))
            {
                foreach (Shell32.FolderItem item in items)
                    yield return item.Path;

                yield break;
            }
        }

        private static IEnumerable<Shell32.FolderItems> GetExplorerSelectedItems(int hwnd)
        {
            return from InternetExplorer window in new ShellWindows()
                where window.HWND == hwnd
                select ((Shell32.IShellFolderViewDual2) window.Document).SelectedItems();
        }

        #region P/Invoke
        [DllImport("user32.dll")]
        private static extern int GetForegroundWindow();
        #endregion
    }
}
