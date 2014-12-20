using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MMK.Marking.Representation
{
    [Serializable]
    public partial class HashTagModel : SortedSet<HashTag>
    {
        public HashTagModel()
            : base(new HashTag.Comparer())
        { }

        public HashTagModel(IEnumerable<HashTag> collection)
            : base(collection, new HashTag.Comparer())
        { }

        protected HashTagModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public bool IsEmpty()
        {
            return Count == 0;
        }

        public override string ToString()
        {
            return string.Join(" ", this);
        }

        public static HashTagModel operator +(HashTagModel lModel, IEnumerable<HashTag> rModel)
        {
            lModel.UnionWith(rModel);
            return lModel;
        }

        public static HashTagModel operator -(HashTagModel lModel, IEnumerable<HashTag> rModel)
        {
            lModel.ExceptWith(rModel);
            return lModel;
        }

        public static HashTagModel operator +(HashTagModel model, HashTag hashTag)
        {
            model.Add(hashTag);
            return model;
        }

        public static HashTagModel operator -(HashTagModel model, HashTag hashTag)
        {
            model.Remove(hashTag);
            return model;
        }

        [Serializable]
        public new class Comparer : HashTag.Comparer, IComparer<HashTagModel>
        {
            protected Comparer(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            public Comparer(bool isDesc = false)
                : base(isDesc)
            {
            }

            public int Compare(HashTagModel x, HashTagModel y)
            {
                var xCount = x == null ? 0 : x.Count;
                var yCount = y == null ? 0 : y.Count;

                if (xCount == yCount)
                    return xCount > 0 ? CompareIfBothHasEqualPatternCount(x, y) : Equal;

                return xCount > yCount ? Higher : Lower;
            }

            private int CompareIfBothHasEqualPatternCount(HashTagModel x, HashTagModel y)
            {
                if (x.SetEquals(y))
                    return Equal;

                var compare = Equal;

                using (var xEnumer = x.GetEnumerator())
                using (var yEnumer = y.GetEnumerator())
                {
                    while (xEnumer.MoveNext())
                    {
                        yEnumer.MoveNext();
                        compare = Compare(xEnumer.Current, yEnumer.Current);
                        if (compare != Equal)
                            return compare;
                    }
                }

                return compare;
            }
        }
    }
}