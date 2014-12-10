using System;
using System.Runtime.Serialization;

namespace MMK.Marking
{
    [Serializable]
    public partial class KeyHashTag : HashTag
    {
        [NonSerialized]
        public static readonly HashTag Unchecked = new HashTag("unch");

        public KeyHashTag(Key key, KeyNotation notation = KeyNotation.Default)
        {
            Key = key;
            Notation = notation;
        }

        public Key Key { get; private set; }

        public override string TagValue
        {
            get { return Key.ToString(Notation); }
        }

        public KeyNotation Notation
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            var hashTag = obj as KeyHashTag;

            if (hashTag == null) return false;

            return hashTag == this || Equals(hashTag);
        }

        protected override bool Equals(HashTag other)
        {
            var hashTag = other as KeyHashTag;
            if (ReferenceEquals(hashTag,null)) return false;
            return ReferenceEquals(hashTag,this) || Equals(hashTag);
        }

        private bool Equals(KeyHashTag other)
        {
            return other !=null && Equals(Key, other.Key) && Notation == other.Notation;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0)*397) ^ (int) Notation;
            }
        }

        public static implicit operator KeyHashTag(Key key)
        {
            return new KeyHashTag(key);
        }

        #region Serialization

        protected KeyHashTag(SerializationInfo info, StreamingContext context)
            :this((Key)info.GetValue("Key",typeof(Key)),(KeyNotation)info.GetValue("Notation",typeof(KeyNotation)))
        { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Key",Key);
            info.AddValue("Notation",Notation);
        }

        #endregion
    }
}
