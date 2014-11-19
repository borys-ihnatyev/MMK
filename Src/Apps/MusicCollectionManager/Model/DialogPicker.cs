using System.Collections.Generic;
using System.Windows.Forms;

namespace MMK.MusicCollectionManager.Model
{
    public class DialogPicker
    {
        private static FolderBrowserDialog folderBrowser;
        private static OpenFileDialog fileDialog;

        public static string PickFolder()
        {
            folderBrowser = folderBrowser ?? new FolderBrowserDialog();
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
