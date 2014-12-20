using System;
using System.Diagnostics.Contracts;
using System.IO;
using MMK.Marking.Representation;

namespace MMK.MagicPlaylist.Models
{
    public class PatternFileFilter
    {
        public PatternFileFilter(HashTagModel hashTagModel)
        {
            if(hashTagModel == null)
                throw new ArgumentNullException("hashTagModel");
            Contract.EndContractBlock();

            Pattern = hashTagModel;
        }

        public PatternFileFilter():this(new HashTagModel())
        {
            
        }

        public HashTagModel Pattern { get; set; }

        public bool Match(string filePath)
        {
            if (Pattern.Count == 0)
                return true;

            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var hashTagModel = HashTagModel.Parser.All(fileName);
            return hashTagModel.IsSupersetOf(Pattern);
        }
    }
}