﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SHDocVw;

// ReSharper disable once CheckNamespace
namespace System.Windows
{
    public class Explorer
    {
        public static IEnumerable<string> GetForegroundSelectedItemsPaths()
        {
            var foregroundWindowHwnd = GetForegroundWindow();
            return GetSelectedItemsPaths(foregroundWindowHwnd);
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