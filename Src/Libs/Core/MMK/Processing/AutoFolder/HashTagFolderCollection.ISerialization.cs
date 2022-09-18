using System.Linq;
using System.Runtime.Serialization;

namespace MMK.Processing.AutoFolder
{
    partial class HashTagFolderCollection
    {
        protected HashTagFolderCollection(SerializationInfo info, StreamingContext context)
            : this()
        {
            var items = (SerializationItem[])info.GetValue("Items", typeof(SerializationItem[]));
            foreach (var item in items)
                Add(item.Folder, item.ModelString, item.Priority);
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var items = patternFolderDictionary
                .Select(pair => new SerializationItem(pair.Value, pair.Key.Model.ToString(),pair.Key.Priority))
                .ToArray();
            info.AddValue("Items", items, typeof(SerializationItem[]));
        }
    }
}
