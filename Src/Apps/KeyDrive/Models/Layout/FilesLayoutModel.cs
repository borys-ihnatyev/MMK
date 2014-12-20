using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using MMK.KeyDrive.Models.Holders;
using MMK.Marking;
using MMK.Marking.Representation;

namespace MMK.KeyDrive.Models.Layout
{
    [Serializable]
    public class FilesLayoutModel : ISerializable
    {
        public FilesLayoutModel(string rootDir)
        {
            RootHolder = new DirectoryHolder(rootDir);
        }

        public DirectoryHolder RootHolder { get; private set; }

        public DirectoryInfo[] this[HashTagModel hashTagModel]
        {
            get
            {
                if (hashTagModel == null)
                    throw new ArgumentNullException("hashTagModel");
                Contract.EndContractBlock();

                return GetOrCreateDirectory(hashTagModel);
            }
        }

        private DirectoryInfo[] GetOrCreateDirectory(IEnumerable<HashTag> hashTagModel)
        {
            if(hashTagModel == null)
                throw new ArgumentNullException("hashTagModel");
            Contract.Ensures(Contract.Result<DirectoryInfo[]>().Length != 0);
            Contract.EndContractBlock();

            var keySet = GetHashTagModelKeys(hashTagModel);

            if (keySet.Count == 0)
                return new[] { RootHolder.Info as DirectoryInfo };

            return keySet
                .Distinct(CircleOfFifths.ParalelEqualityComparer)
                .Select(CreateKeyDirectory)
                .ToArray();
        }

        private static List<Key> GetHashTagModelKeys(IEnumerable<HashTag> hashTagModel)
        {
            return hashTagModel
                .OfType<KeyHashTag>()
                .Select(h => h.Key)
                .ToList();
        }

        [Pure]
        private DirectoryInfo CreateKeyDirectory(Key key)
        {
            var path = DirectoryPath(key);
            
            return Directory.Exists(path) 
                ? new DirectoryInfo(path) 
                : Directory.CreateDirectory(path);
        }

        [Pure]
        private string DirectoryPath(Key key)
        {
            return Path.Combine(RootHolder.Info.FullName, DirectoryName(key));
        }

        [Pure]
        protected virtual string DirectoryName(Key key)
        {
            var minorKey = key.IsMoll() ? key : CircleOfFifths.GetParalel(key);
            var majorKey = key.IsDur() ? key : CircleOfFifths.GetParalel(key);

            return String.Format("{0} {1},{2}", 
                minorKey.ToString(KeyNotation.CamelotWithoutTone), 
                minorKey.Note,
                majorKey.Note
            );
        }

        public void CleanUp()
        {
            ((DirectoryInfo) RootHolder.Info).EnumerateDirectories("*", SearchOption.AllDirectories)
                .Where(d => !d.EnumerateFiles().Any())
                .ForEach(d => d.Delete());
        }

        #region Serialization

        private const string RootDirName = "RootDir";

        public FilesLayoutModel(SerializationInfo info, StreamingContext context)
            :this(info.GetString(RootDirName))
        {
            
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(RootDirName,RootHolder.Info.FullName);
        }

        #endregion
    }
}