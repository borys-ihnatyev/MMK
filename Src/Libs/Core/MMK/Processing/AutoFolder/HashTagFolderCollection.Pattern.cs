using System.Collections.Generic;
using MMK.Marking.Representation;

namespace MMK.Processing.AutoFolder
{
    partial class HashTagFolderCollection
    {
        public class Pattern 
        {
            public Pattern(HashTagModel model, int priority)
            {
                Model = model;
                Priority = priority;
            }

            public HashTagModel Model { get; private set; }

            public int Priority { get; private set; }

            public class Comparer : HashTagModel.Comparer, IComparer<Pattern>
            {
			    public Comparer() : base(true)
			    {

			    }

			    public int Compare(Pattern x, Pattern y)
			    {
			        if (x != null && y == null)
			            return Higher;

                    if (x == null && y != null)
                        return Lower;

			        if (x.Priority > y.Priority)
			            return Higher;

			        if (x.Priority < y.Priority)
			            return Lower;

			        return Compare(x.Model, y.Model);

			    }
            }
        }
    }
}
