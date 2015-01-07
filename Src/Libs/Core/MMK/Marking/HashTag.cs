using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MMK.Marking
{
    [XmlInclude(typeof(KeyHashTag))]
    [Serializable]
    public partial class HashTag : IHashTag, ISerializable
    {
        [NonSerialized]
        public static readonly string Hash = "#";

        private readonly string tagValue;

        public HashTag(string tagValue)
        {
            this.tagValue = tagValue.Replace(" ", "").ToLower();;
        }

        public HashTag()
            : this(string.Empty)
        { }

        public virtual string TagValue
        {
            get { return tagValue; }
        }

        public bool IsEmpty()
        {
            return TagValue == string.Empty;
        }

        public override bool Equals(object obj)
        {
            var hashTag = obj as HashTag;

            if (hashTag == null) return false;
            if (this == hashTag) return true;

            return Equals(hashTag);
        }

        protected virtual bool Equals(HashTag other)
        {
            return other != null && string.Equals(tagValue, other.tagValue);
        }

        public override int GetHashCode()
        {
            return (tagValue != null ? tagValue.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return Hash + TagValue;
        }

        #region Serialization

        protected HashTag(SerializationInfo info, StreamingContext context)
            : this(info.GetString("TagValue"))
        { }
        
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TagValue",tagValue);
        }
        #endregion

        public static implicit operator string(HashTag hashTag)
        {
            return hashTag.ToString();
        }

        [Serializable]
        public class Comparer : IComparer<HashTag>, IComparer<KeyHashTag>, ISerializable
        {
            [NonSerialized]
            protected readonly int Higher;

            [NonSerialized]
            protected readonly int Lower;

            [NonSerialized]
            protected readonly int Equal;

            public Comparer(bool isDesc = false)
            {
                if (isDesc)
                {
                    Higher = -1;
                    Lower = 1;
                    Equal = 0;
                }
                else
                {
                    Higher = 1;
                    Lower = -1;
                    Equal = 0;
                }    
            }

            protected Comparer(SerializationInfo info, StreamingContext context)
                : this(info.GetBoolean("IsDesc"))
            {

            }

            public int Compare(HashTag x, HashTag y)
            {
                if (Equals(x, y)) return Equal;

                if (x == null) return Higher;
                if (y == null) return Lower;

                if (x.Equals(y)) return Equal;

                if (string.IsNullOrWhiteSpace(x.TagValue)) return Higher;
                if (string.IsNullOrWhiteSpace(y.TagValue)) return Lower;

                var xKeyHashTag = x as KeyHashTag;
                var yKeyHashTag = y as KeyHashTag;

                if (xKeyHashTag != null || yKeyHashTag != null) 
                    return Compare(xKeyHashTag, yKeyHashTag);

                var compare = string.Compare(x, y, true);

                if (compare == 1)
                    return Higher;

                return compare == -1 ? Lower : Equal;
            }

            public int Compare(KeyHashTag x, KeyHashTag y)
            {
                if (Equals(x, y)) return Equal;

                if (x == null) return Higher;
                if (y == null) return Lower;

                if (x.Key == y.Key) return Equal;

                foreach (var key in CircleOfFifths.AllKeys)
                    if (key == x.Key)
                        return Lower;
                    else
                        if (key == y.Key)
                            return Higher;
                
                throw new InvalidOperationException("Must allways return in code above");
            }

            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("IsDesc", Higher < Lower);
            }
        }
    }
}
