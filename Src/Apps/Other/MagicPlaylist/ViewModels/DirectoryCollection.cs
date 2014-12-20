using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace MMK.MagicPlaylist.ViewModels
{
    public class DirectoryCollection : ObservableCollection<string>
    {
        protected override void InsertItem(int index, string path)
        {
            path = PathExtension.Normalize(path);

            if (!CanAddDirectory(path)) return;
            
            var children = GetChildrenFor(path).ToList();
            base.InsertItem(index, path);
            children.ForEach(dir => Remove(dir));
        }

        private bool CanAddDirectory(string path)
        {
            return Directory.Exists(path)
                   && !Contains(path)
                   && !IsAnySubdirectory(path);
        }

        private bool IsAnySubdirectory(string path)
        {
            return this
                .Where(dir => !dir.Equals(path, StringComparison.OrdinalIgnoreCase))
                .Any(dir => path.StartsWith(dir, StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<string> GetChildrenFor(string path)
        {
            return this.Where(dir => dir.Contains(path));
        } 

        public void Sanitize()
        {
            this.Where(dir => !Directory.Exists(dir))
                .ToList()
                .ForEach(dir => Remove(dir));
        }
    }
}