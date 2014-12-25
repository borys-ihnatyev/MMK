using System;
using System.Diagnostics.Contracts;

namespace MMK.Marking
{
    public partial class HashTag
    {
        public class Parser
        {

            /// <summary>
            /// Parses first hashtag occurence in target string
            /// </summary>
            /// <param name="value">target string</param>
            /// <param name="beginsIndex">position from witch start parse</param>
            /// <returns>Hash tag entry in target string or null if no hash tags</returns>
            public static Entry First(string value, int beginsIndex = 0)
            {   
                if (value.Length == 0 && beginsIndex == 0)
                    return null;

                value = value.Substring(beginsIndex);

                var hashIndex = value.IndexOf(Hash, StringComparison.Ordinal);

                if (hashIndex == -1) 
                    return null;

                value = value.Substring(hashIndex);
                value = FirstWord(value);

                if (value.Length == 1)
                    return null;
                
                var hashTagValue = value.Substring(1);

                var hashTag = BuildHashTag(hashTagValue);

                return new Entry(hashTag, beginsIndex + hashIndex, value.Length);
            }

            protected static string FirstWord(string value)
            {
                var spaceIndex = value.IndexOf(' ');

                return spaceIndex == -1 
                    ? value 
                    : value.Substring(0, spaceIndex);
            }

            private static HashTag BuildHashTag(string value)
            {
                var hashTag = new HashTag(value);
                var keyHashTag = KeyHashTag.Parser.ToKeyHashTag(hashTag);
                
                if (keyHashTag != null)
                    hashTag = keyHashTag;

                return hashTag;
            }
        }
    }
}