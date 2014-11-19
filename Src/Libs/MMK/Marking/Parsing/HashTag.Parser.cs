using System;

namespace MMK.Marking
{
    public partial class HashTag
    {
        public class Parser
        {
            /// <summary>
            /// Wire hash tag in target string
            /// </summary>
            /// <param name="value">target string</param>
            /// <param name="hashTag">hash tag to write</param>
            /// <returns>target string with added hash tag</returns>
            public static string Add(string value, HashTag hashTag)
            {
                value = value.Trim();
                value += " " + hashTag;
                return value;
            }

            /// <summary>
            /// Parses first hashtag occurence in target string
            /// </summary>
            /// <param name="value">target string</param>
            /// <param name="startIndex">position from witch start parse</param>
            /// <returns>Hash tag entry in target string or null if no hash tags</returns>
            public static Entry First(string value, int startIndex = 0)
            {
                if (value.Length == 0 && startIndex == 0)
                    return null;

                if (startIndex >= value.Length)
                    throw new ArgumentException();

                value = value.Substring(startIndex);

                int hashIndex = value.IndexOf(Hash, StringComparison.Ordinal);

                if (hashIndex <= -1) return null;

                int lastIndex = hashIndex + 1;

                for (; lastIndex < value.Length; lastIndex++)
                    if (char.IsWhiteSpace(value, lastIndex))
                    {
                        --lastIndex;
                        break;
                    }

                if (lastIndex == value.Length) --lastIndex;

                if (lastIndex <= hashIndex) return null;

                int hashTagPureLength = lastIndex - hashIndex;
                var hashTag = new HashTag(value.Substring(hashIndex + 1, hashTagPureLength));
                var keyHashTag = KeyHashTag.Parser.ToKeyHashTag(hashTag);
                if (keyHashTag != null)
                    hashTag = keyHashTag;

                return new Entry(hashTag, hashIndex + startIndex, hashTagPureLength + 1);
            }
        }
    }
}
