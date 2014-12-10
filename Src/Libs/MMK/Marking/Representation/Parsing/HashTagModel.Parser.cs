using System;

namespace MMK.Marking.Representation
{
    public partial class HashTagModel
    {
        public class Parser
        {
            /// <summary>
            /// Parsing all hash tag occurence in target string 
            /// </summary>
            /// <param name="value">target string</param>
            /// <returns>hash tag model</returns>
            public static HashTagModel All(string value)
            {
                return All(ref value);
            }

            /// <summary>
            /// Parsing all hash tag occurence in target string 
            /// and removes it from target string
            /// </summary>
            /// <param name="value">target string</param>
            /// <returns>hash tag model</returns>
            public static HashTagModel All(ref string value)
            {
                var hashTagModel = new HashTagModel();

                var hashTagEntry = HashTag.Parser.First(value);

                while (hashTagEntry != null)
                {
                    hashTagModel.Add(hashTagEntry.HashTag);
                    value = value.Remove(hashTagEntry.Index, hashTagEntry.Length);
                    hashTagEntry = HashTag.Parser.First(value);
                }

                value = value.Trim();
                return hashTagModel;
            }

            private static bool IsHashTag(string value)
            {
                return value.StartsWith(HashTag.Hash) && value.Length > 1;
            }
        }
    }
}
