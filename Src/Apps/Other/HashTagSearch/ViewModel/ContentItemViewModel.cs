using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MMK.Taging;
using System.Windows.Input;
using MMK.Taging.Parsing;
using MMK.Wpf.ViewModel;

namespace MMK.HashTagSearch.ViewModel
{
    public class ContentItemViewModel : NotifyPropertyChangedBase
    {
        public ContentItemViewModel(string path)
        {
            this.path = path;
            this.name = System.IO.Path.GetFileNameWithoutExtension(path);
            this.directory = System.IO.Path.GetDirectoryName(path);
            this.extension = System.IO.Path.GetExtension(path);
            this.clearName = this.name;
            StartHashTagParsingTask();
        }

        private string name;

        private string path;

        private string directory;

        private string extension;
        
        private string clearName;

        private void StartHashTagParsingTask() 
        {
            new Task(() =>
            {
                var keyEntry = KeyHashTagParser.ParseFirst(this.name);
                if (keyEntry != null)
                {
                    var clearedName = this.name.Remove(keyEntry.Index, keyEntry.Length);
                    ClearName = clearedName;
                    KeyString = keyEntry.HashTag;
                }
                else
                {
                    KeyString = "#unch";
                }
            }).Start();
        }

        protected void OnPathChanged(string oldPath)
        {
            NotifyPropertyChanged("Path");
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            Directory = System.IO.Path.GetDirectoryName(path);
            Extension = System.IO.Path.GetExtension(path);
            
            if (PathChanged != null)
                PathChanged(this, new ChangedEventArgs<string>(oldPath, path));
        }

        public string Path
        {
            get { return path; }
            set
            {
                if (value != path)
                {
                    var oldPath = path;
                    path = value;
                    OnPathChanged(oldPath);
                }
            }
        }

        public string Directory 
        {
            get { return directory; }
            private set
            {
                if (value != directory)
                {
                    directory = value;
                    NotifyPropertyChanged("Directory");
                }
            }
        }

        public string Name
        {
            get { return name; }
            private set
            {
                if (value != name)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string ClearName
        {
            get { return clearName; }
            private set
            {
                if (value != clearName)
                {
                    clearName = value;
                    NotifyPropertyChanged("ClearName");
                }
            }
        }

        private string keyString;

        public string KeyString
        {
            get { return keyString; }
            private set 
            {
                if (value != keyString)
                {
                    keyString = value;
                    NotifyPropertyChanged("KeyString");
                }
            }
        }
        
        public string Extension
        {
            get { return extension; }
            private set 
            {
                if (value != extension)
                {
                    extension = value;
                    NotifyPropertyChanged("Extension");
                }
            }
        }

        public event EventHandler<ChangedEventArgs<string>> PathChanged;

        public static implicit operator ContentItemViewModel(string filePath)
        {
            var fileItemViewModel = new ContentItemViewModel(filePath);
            return fileItemViewModel;
        }
        public static implicit operator string(ContentItemViewModel fileItemViewModel)
        {
            return fileItemViewModel.Path;
        }
    }
}
