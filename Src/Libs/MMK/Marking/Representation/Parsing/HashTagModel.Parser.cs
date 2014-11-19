namespace MMK.Marking.Representation
{
    public partial class HashTagModel
    {
        public class Parser
        {
            /// <summary>
            /// Write all hash tags from hash tag model in target string
            /// </summary>
            /// <param name="value">target string</param>
            /// <param name="hashTagModel">hash tag model</param>
            /// <returns>target string with added hash tags from hash tag model</returns>
            public static string Add(string value, HashTagModel hashTagModel)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return hashTagModel.ToString();

                if (hashTagModel.IsEmpty()) return value;

                value = value.Trim();
                value += " " + hashTagModel;

                return value;
            }

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
        }
    }
}
