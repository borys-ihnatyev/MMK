using System.Collections.Generic;

namespace System.Windows.Forms
{
    public class DialogPicker
    {
        private static FolderBrowserDialog folderBrowser;
        private static OpenFileDialog fileDialog;

        public static string PickFolder(string rootFolder = null)
        {
            folderBrowser = folderBrowser ?? new FolderBrowserDialog();
            if (rootFolder != null)
                folderBrowser.SelectedPath = rootFolder;
            return folderBrowser.ShowDialog() == DialogResult.OK ? folderBrowser.SelectedPath : null;
        }

        public static string PickFile()
        {
            fileDialog = fileDialog ?? new OpenFileDialog();
            return fileDialog.ShowDialog() == DialogResult.OK ? fileDialog.FileName : null;
        }

        public static IEnumerable<string> PickFiles()
        {
            fileDialog = fileDialog ?? new OpenFileDialog();
            return fileDialog.ShowDialog() == DialogResult.OK ? fileDialog.FileNames : null;
        }
    }
}