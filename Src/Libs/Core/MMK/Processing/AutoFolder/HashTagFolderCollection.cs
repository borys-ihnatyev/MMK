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
    public partial class HashTagFolderCollection : IEnumerable<KeyValuePair<HashTagModelPattern, string>>,
        ISerializable
    {
        [NonSerialized] private readonly SortedDictionary<HashTagModelPattern, string> patternFolderDictionary;

        [NonSerialized] private readonly SortedDictionary<string, MusicFolder> pathMusicFolderDictionary;

        public HashTagFolderCollection()
        {
            patternFolderDictionary = new SortedDictionary<HashTagModelPattern, string>(new HashTagModelPattern.Comparer());
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

            var pattern = new HashTagModelPattern(model, priority);

            if (patternFolderDictionary.ContainsKey(pattern))
                throw new PatternAlreadyExistsException();

            patternFolderDictionary.Add(pattern, path);
        }

        public MusicFolder.ResultInfo MoveFile(string filePath, HashTagModel fileHashTagModel)
        {
            var matchPath = GetMatchPath(fileHashTagModel);
            var matchFileMusicFolder = pathMusicFolderDictionary[matchPath];

            var isDeprecated = fileHashTagModel.Contains(MusicFolder.DeprecatedHashTag);

            return isDeprecated
                ? matchFileMusicFolder.DeprecatedFolder.MoveFile(filePath, true) 
                : matchFileMusicFolder.MoveFile(filePath);
        }

        [Pure]
        public IEnumerable<string> Folders
        {
            get { return pathMusicFolderDictionary.Keys; }
        }

        [Pure]
        public IEnumerable<HashTagModelPattern> GetPatternsForFolder(string path)
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
        public IEnumerator<KeyValuePair<HashTagModelPattern, string>> GetEnumerator()
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