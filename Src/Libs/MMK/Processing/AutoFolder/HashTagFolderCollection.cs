using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using MMK.Marking.Representation;

namespace MMK.Processing.AutoFolder
{
    [Serializable]
    public partial class HashTagFolderCollection : IEnumerable<KeyValuePair<HashTagFolderCollection.Pattern, string>>,
        ISerializable
    {
        [NonSerialized] private readonly SortedDictionary<Pattern, string> patternFolderDictionary;

        [NonSerialized] private readonly SortedDictionary<string, MusicFolder> pathMusicFolderDictionary;

        public HashTagFolderCollection()
        {
            patternFolderDictionary = new SortedDictionary<Pattern, string>(new Pattern.Comparer());
            pathMusicFolderDictionary = new SortedDictionary<string, MusicFolder>();
        }

        public void Add(string path, string pattern, int priority = 0)
        {
            Contract.Requires(path != null);
            Contract.EndContractBlock();

            Add(path, HashTagModel.Parser.All(pattern), priority);
        }

        public void Add(string path, HashTagModel model, int priority = 0)
        {
            Contract.Requires(path != null);
            Contract.EndContractBlock();

            path = PathExtension.Normalize(path);

            if (!pathMusicFolderDictionary.ContainsKey(path))
            {
                var musicFolder = new MusicFolder(path);
                pathMusicFolderDictionary[path] = musicFolder;
            }

            var pattern = new Pattern(model, priority);

            if (patternFolderDictionary.ContainsKey(pattern))
                throw new PatternAlreadyExistsException();

            patternFolderDictionary.Add(pattern, path);
        }

        public MusicFolder.ResultInfo MoveFile(string filePath, HashTagModel fileHashTagModel)
        {
            var currentFileMusicFolder = GetCurrentMusicFolder(filePath);
            var matchPath = GetMatchPath(fileHashTagModel);
            var matchFileMusicFolder = pathMusicFolderDictionary[matchPath];

            return currentFileMusicFolder == null 
                ? matchFileMusicFolder.MoveFile(filePath) 
                : matchFileMusicFolder.MoveFile(filePath, currentFileMusicFolder);
        }

        [Pure]
        public IEnumerable<string> Folders
        {
            get { return pathMusicFolderDictionary.Keys; }
        }

        [Pure]
        public IEnumerable<Pattern> GetPatternsForFolder(string path)
        {
            return from patternFolder in patternFolderDictionary
                where patternFolder.Value == path
                select patternFolder.Key;
        }

        [Pure]
        public string GetMatchPath(HashTagModel model)
        {
            try
            {
                return patternFolderDictionary.First(pair => model.IsSupersetOf(pair.Key.Model)).Value;
            }
            catch (InvalidOperationException)
            {
                throw new NoMatchPatternException();
            }
        }

        [Pure]
        public bool HasPattern(HashTagModel model)
        {
            return patternFolderDictionary.Keys.Any(pattern => pattern.Model == model);
        }

        [Pure]
        public bool Match(HashTagModel model)
        {
            return patternFolderDictionary.Keys.Any(pattern => model.IsSupersetOf(pattern.Model));
        }

        [Pure]
        private MusicFolder GetCurrentMusicFolder(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
                return null;

            return pathMusicFolderDictionary
                    .Select(p => p.Value)
                    .FirstOrDefault(folder => folder.HasFile(fileInfo));
        }

        [Pure]
        public IEnumerator<KeyValuePair<Pattern, string>> GetEnumerator()
        {
            return patternFolderDictionary.GetEnumerator();
        }
        
        [Pure]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Serializable]
        public class PatternAlreadyExistsException : Exception
        {
        }

        [Serializable]
        public class NoMatchPatternException : Exception
        {
        }
    }
}