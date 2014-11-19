using System;
using MMK.Marking.Parsing;

namespace MMK.Marking
{
    public partial class HashTag
    {
        public class Entry : IHashTagEntry
        {
            private readonly HashTag hashTag;

            public Entry(HashTag hashTag, int index, int length)
            {
                if (index < 0) throw new ArgumentException("cant be less then 0", "index");
                if (length < 1) throw new ArgumentException("cant be less then 1", "length");
                if (hashTag == null)
                    throw new ArgumentNullException("hashTag","for creating empty hash tag use const NoHashTagEntry");
                Index = index;
                Length = length;
                this.hashTag = hashTag;
            }

            /// <summary>
            /// For creating empty hashtag
            /// </summary>
            private Entry()
            {
                Index = -1;
                Length = 0;
                hashTag = null;
            }

            public int Index { get; private set; }
            public int Length { get; private set; }

            public bool IsNotEmpty()
            {
                return this != NoHashTagEntry;
            }

            public static readonly Entry NoHashTagEntry = new Entry();

            HashTag IHashTagEntry.HashTag
            {
                get { return hashTag; }
            }

            public HashTag HashTag
            {
                get { return hashTag; }
            }
        }
    }
}
