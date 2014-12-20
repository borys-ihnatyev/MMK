using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MMK.Utils;

namespace MMK.MagicPlaylist.Models
{
    public class MagicPlaylistGenerator
    {
        public IEnumerable<string> Generate()
        {
            return new string[0];
        }

        public async Task<IEnumerable<string>> GenerateAsync()
        {
            return await Task.Run(() => Generate());
        } 

        public void Add(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Not found", path);
            if (!FileExtensionParser.IsMp3(path))
                throw new UnsupportedFormatException();
        }

        public class UnsupportedFormatException : Exception
        {
        }
    }
}