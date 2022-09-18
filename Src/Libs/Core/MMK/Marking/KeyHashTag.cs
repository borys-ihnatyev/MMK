using System;
using System.Runtime.Serialization;

namespace MMK.Marking
{
    [Serializable]
    public partial class KeyHashTag : HashTag
    {
        [NonSerialized]
        public static readonly HashTag Unchecked = new HashTag("unch");

        public KeyHashTag(Key key, string metaValue, KeyNotation notation = KeyNotation.Default)
            :base(key.ToString(notation),metaValue)
        {
            Key = key;
            Notation = notation;
        }

        public KeyHashTag(Key key, KeyNotation notation = KeyNotation.Default) : this(key, string.Empty, notation)
        {
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
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((KeyHashTag) obj);
        }

        protected override bool Equals(HashTag other)
        {
            if (ReferenceEquals(other,null)) return false;
            if (ReferenceEquals(other, this)) return true;
            if (other.GetType() != GetType()) return false;
            return Equals((KeyHashTag)other);
        }

        protected bool Equals(KeyHashTag other)
        {
            return Equals(Key, other.Key) && Notation == other.Notation;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Key != null ? Key.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) Notation;
                return hashCode;
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
