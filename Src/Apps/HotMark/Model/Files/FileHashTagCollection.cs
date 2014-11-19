using System.Collections;
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
            if (files.Count == 0)
                return new HashTagModel();

            var jointHashTagModel = new HashTagModel(files.First().HashTagModel);

            foreach (var fileItem in files)
            {
                jointHashTagModel.IntersectWith(fileItem.HashTagModel);
                if (jointHashTagModel.Count == 0)
                    break;
            }

            return jointHashTagModel;
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