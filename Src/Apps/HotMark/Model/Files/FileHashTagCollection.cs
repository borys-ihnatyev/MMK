﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MMK.Marking.Representation;

namespace MMK.HotMark.Model.Files
{
    public sealed class FileHashTagCollection : IEnumerable<FileHashTagModel>
    {
        private readonly HashSet<FileHashTagModel> files = new HashSet<FileHashTagModel>();

        private bool wasCahnged = true;

        private HashTagModel conjointHashTagModel;

        public FileHashTagCollection()
        {
        }

        public FileHashTagCollection(IEnumerable<string> paths)
        {
            Build(paths);
        }

        private void Build(IEnumerable<string> paths)
        {
            foreach (var path in paths)
                Add(path);
        }

        public void Add(string path)
        {
            if (File.Exists(path))
                AddFile(path);
            else if (Directory.Exists(path))
                AddFilesInDirectory(path);
            else
                throw new FileNotFoundException("no such file or directory", path);
        }

        private void AddFilesInDirectory(string path)
        {
            foreach (var file in Directory.EnumerateFiles(path, "*.mp3", SearchOption.AllDirectories))
                AddFile(file);
        }

        private void AddFile(string path)
        {
            files.Add(new FileHashTagModel(path));
            wasCahnged = true;
        }

        public int Count
        {
            get { return files.Count; }
        }


        public HashTagModel ConjointHashTagModel
        {
            get
            {
                if (wasCahnged)
                {
                    conjointHashTagModel = MakeConjointHashTagModel();
                    wasCahnged = false;
                }

                return conjointHashTagModel;
            }
        }

        private HashTagModel MakeConjointHashTagModel()
        {
            return HashTagModel.Conjoint(files.Select(file => file.HashTagModel));
        }

        public bool HasEqualHashTagModel
        {
            get { return CalcMaxHashTagCount() == ConjointHashTagModel.Count; }
        }

        private int CalcMaxHashTagCount()
        {
            return files.Count == 0
                ? 0
                : files.Select(fileItem => fileItem.HashTagModel.Count).Max();
        }


        public IEnumerable<string> GetPaths()
        {
            return files.Select(file => file.Path);
        }

        public IEnumerator<FileHashTagModel> GetEnumerator()
        {
            return files.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}