using System;
using System.Diagnostics.Contracts;
using System.IO;
using MMK.Marking.Representation;

namespace MMK.Processing
{
    public class CoverArtFactory
    {
        protected readonly DirectoryInfo CoverArtsDir;

        public CoverArtFactory(string directory)
        {
            if(String.IsNullOrWhiteSpace(directory))
                throw new ArgumentException("invalid directory name","directory");
            Contract.EndContractBlock();

            CoverArtsDir = new DirectoryInfo(directory);

            if (CoverArtsDir.Exists) return;
            
            CoverArtsDir.Create();
            CoverArtsDir.Refresh();            
        }

        public virtual string RetriveImagePath(HashTagModel hashTagModel)
        {
            return null;
        }
    }
}