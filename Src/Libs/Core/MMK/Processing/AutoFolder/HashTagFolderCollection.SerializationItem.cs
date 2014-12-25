using System;

namespace MMK.Processing.AutoFolder
{
    partial class HashTagFolderCollection
    {
		[Serializable]
        private class SerializationItem
        {
            public SerializationItem(string folder, string modelString, int priority)
            {
                Folder = folder;
                ModelString = modelString;
                Priority = priority;
            }

		    public readonly string Folder;
		    public readonly string ModelString;
		    public readonly int Priority;
        }
    }
}
