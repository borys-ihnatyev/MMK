namespace MMK.Marking
{
    public partial class KeyHashTag
    {
        public new class Entry : HashTag.Entry
        {
            public Entry(KeyHashTag hashTag, int index, int length)
                : base(hashTag, index, length)
            {
            }

            public new KeyHashTag HashTag
            {
                get { return (KeyHashTag) base.HashTag; }
            }
        }
    }
}
