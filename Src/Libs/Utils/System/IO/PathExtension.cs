using System.Diagnostics.Contracts;

namespace System.IO
{
    public static class PathExtension
    {
        public static string Normalize(string path)
        {
            Contract.Requires(path != null);
            Contract.EndContractBlock();

            path = Path.GetFullPath(path);
            return Path.GetFullPath(new Uri(path).LocalPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}