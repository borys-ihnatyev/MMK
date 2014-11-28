using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable once CheckNamespace

namespace MMK.Marking.Representation
{
    public partial class TrackNameModel
    {
        public class Parser
        {
            private Parser(string trackName)
            {
                this.trackName = trackName;
            }

            #region Constants

            private static readonly string[] FeatureArtistsSeparators =
            {
                " feat. ",
                " feat.",
                " feat ",
                " ft. ",
                " ft.",
            };

            private static readonly string[] ArtistsSeparators = 
            {

                " feat. ",
                " feat.",
                " feat ",
                " ft. ",
                " ft.",
                " , ",
                ", ",
                " ,",
                " and ",
                " & ",
                "& "
            };

            private static readonly string[] RemixTypesPrefix =
            {
                "original",
                "extended",
                "club",
                "radio",
                "dub",
                "instrumental",
                "vox"
            };

            private static readonly string[] RemixTypes =
            {
                "remix",
                "mix",
                "mashup",
                "mash up",
                "edit",
                "bootleg",
                "reboot",
                "version",
                "vox"
            };

            private const string WhiteSpace = " ";
            private const string DoubleWhiteSpace = WhiteSpace + WhiteSpace;

            private const string OpenBracket = "(";
            private const string OpenBracketSpace = OpenBracket + WhiteSpace;

            private const string CloseBracket = ")";
            private const string CloseBracketSpace = WhiteSpace + CloseBracket;

            private const string DownSlash = "_";
            private const string LongSeparator = "–";

            private const string ShortSeparator = "-";

            private const string DoubleShortSeparator = "--";

            private const string ArtistTitleSeparator = " - ";

            #endregion

            private TrackNameModel model;

            #region Sequence Call Methods

            private string trackName;

            private void InitializeTrackNameModel()
            {
                trackName = NormalizeTrackName(trackName).ToLower();

                model.HashTagModel = HashTagModel.Parser.All(ref trackName);
                
                model.AddArtists(ExtractArtists());

                string title; 

                var titleArtists = ExtractTitleAndArtists(out title);

                model.AddArtists(titleArtists);

                model.Title = title;

                model.MixType = ExtractRemixType();
                
                model.AddRemixArtists(ExtractRemixArtists());
            }


            private IEnumerable<string> ExtractArtists()
            {
                var artistsStringLength = trackName.IndexOf(ArtistTitleSeparator, StringComparison.Ordinal);

                if (artistsStringLength < 1)
                    return new[] {Unknown + " Artist"};

                var artistsString = trackName.Substring(0, artistsStringLength);
                var artists = artistsString.Split(ArtistsSeparators, StringSplitOptions.RemoveEmptyEntries);

                trackName = trackName.Substring(artistsStringLength - 1 + ArtistTitleSeparator.Length);

                return artists;
            }

            private IEnumerable<string> ExtractTitleAndArtists(out string title)
            {
                title = ExtractTitle();
                
                var titleArtists = title.Split(FeatureArtistsSeparators,StringSplitOptions.None);

                if (titleArtists.Length <= 1)
                    return new string[0];

                title = titleArtists[0];

                return titleArtists.Skip(1) //skeeps title
                    .SelectMany(ta => ta.Split(ArtistsSeparators, StringSplitOptions.RemoveEmptyEntries))
                    .ToArray();
            }

            private string ExtractTitle()
            {
                if (trackName.Length == 0)
                    throw new InvalidNameFormatException("@title : " + trackName);

                string title;
                
                var titleStringLength = trackName.IndexOf(OpenBracket, StringComparison.Ordinal);
                
                if (titleStringLength > 0)
                {
                    title = trackName.Substring(0, titleStringLength);
                    trackName = trackName.Substring(titleStringLength);                                    
                }
                else
                {
                    title = trackName.Trim();
                    trackName = string.Empty;
                }

                return title;
            }

            private string ExtractRemixType()
            {
                if (trackName.Length == 0)
                    return string.Empty;

                trackName = trackName.Replace(OpenBracket, string.Empty).Trim();
                var remixType = ExtractMatchedRemixType();
                var mixType = ExtractMatchedRemixTypePrefix(string.IsNullOrWhiteSpace(remixType));
                return mixType + WhiteSpace + remixType;
            }

            private string ExtractMatchedRemixType()
            {
                foreach (var remixType in RemixTypes)
                {
                    var indexOfRemixType = trackName.IndexOfAny(
                        RemixTypeRules(remixType),
                        StringComparison.InvariantCultureIgnoreCase
                        );

                    if (indexOfRemixType <= -1) continue;

                    trackName = trackName.Remove(indexOfRemixType);

                    return remixType;
                }

                return string.Empty;
            }

            private static IEnumerable<string> RemixTypeRules(string remixType)
            {
                return new[]
                {
                    WhiteSpace + remixType + WhiteSpace + CloseBracket,
                    WhiteSpace + remixType + CloseBracket,
                    remixType + CloseBracket,
                    WhiteSpace + remixType + WhiteSpace,
                    WhiteSpace + remixType,
                    remixType + WhiteSpace,
                    remixType
                };
            }

            private string ExtractMatchedRemixTypePrefix(bool hasRemixType)
            {
                Func<string, IEnumerable<string>> remixTypePrefixRules;
                if (hasRemixType)
                    remixTypePrefixRules = RemixTypePrefixRules;
                else
                    remixTypePrefixRules = RemixTypePrefixRulesWhenNoRemixType;

                foreach (var remixTypePrefix in RemixTypesPrefix)
                {
                    var indexOfRemixTypePrefix = trackName.IndexOfAny(
                        remixTypePrefixRules(remixTypePrefix),
                        StringComparison.InvariantCultureIgnoreCase
                        );

                    if (indexOfRemixTypePrefix <= -1) continue;

                    trackName = trackName.Remove(indexOfRemixTypePrefix);

                    return remixTypePrefix;
                }

                return string.Empty;
            }

            private static IEnumerable<string> RemixTypePrefixRules(string remixTypePrefix)
            {
                return new[]
                {
                    WhiteSpace + remixTypePrefix + WhiteSpace + CloseBracket,
                    WhiteSpace + remixTypePrefix + CloseBracket,
                    remixTypePrefix + CloseBracket,
                    WhiteSpace + remixTypePrefix + WhiteSpace,
                    WhiteSpace + remixTypePrefix,
                    remixTypePrefix + WhiteSpace, remixTypePrefix
                };
            }

            private static IEnumerable<string> RemixTypePrefixRulesWhenNoRemixType(string remixTypePrefix)
            {
                return new[]
                {
                    WhiteSpace + remixTypePrefix + WhiteSpace,
                    WhiteSpace + remixTypePrefix,
                    remixTypePrefix + WhiteSpace, remixTypePrefix
                };
            }


            private IEnumerable<string> ExtractRemixArtists()
            {
                if (trackName.Length == 0)
                    return new string[0];

                model.ClearRemixArtists();

                var replaceRules = new Dictionary<string, string>
                {
                    {OpenBracket, string.Empty},
                    {CloseBracket, string.Empty}
                };

                trackName = trackName.Replace(replaceRules).Trim();
                return trackName.Split(ArtistsSeparators, StringSplitOptions.RemoveEmptyEntries);
            }

            #endregion

            public TrackNameModel GetTrackNameModel()
            {
                model = new TrackNameModel();
                InitializeTrackNameModel();
                return model;
            }


            public static string NormalizeTrackName(string trackName)
            {
                trackName = Transliteration.Front(trackName);
                trackName = NormalizeCharacters(trackName);
                return trackName;
            }

            private static string NormalizeCharacters(string value)
            {
                return value
                    .Replace(new Dictionary<string, string>
                    {
                        {DownSlash, WhiteSpace},
                        {LongSeparator, ShortSeparator},
                        {DoubleWhiteSpace, WhiteSpace},
                        {DoubleShortSeparator, ShortSeparator},
                        {OpenBracketSpace, OpenBracket},
                        {CloseBracketSpace, CloseBracket}
                    })
                    .Trim();
            }


            public static TrackNameModel TryParse(string trackName)
            {
                try
                {
                    return Parse(trackName);
                }
                catch (InvalidNameFormatException)
                {
                    return null;
                }
            }

            public static TrackNameModel Parse(string trackName)
            {
                var parser = new Parser(trackName);
                return parser.GetTrackNameModel();
            }
        }
    }
}