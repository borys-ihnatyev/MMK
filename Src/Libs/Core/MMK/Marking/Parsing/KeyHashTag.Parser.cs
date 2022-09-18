using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MMK.Marking
{
    public partial class KeyHashTag
    {
        public new class Parser : HashTag.Parser
        {
            #region Const

            public const string HashString = "#";
            private static readonly string[] MinorTones = {"moll", "minor", "min", "m"};
            private static readonly string[] MajorTones = {"dur", "major", "maj"};

            #endregion

            public static Note ToNote(string noteString)
            {
                noteString = noteString.Trim(' ', '-').Replace("#", "is");
                Note note;
                if (Enum.TryParse(noteString, true, out note))
                    return note;
                throw new NoteNotFoundException();
            }

            public static Tone ToTone(string tone)
            {
                tone = tone.ToLower();
                switch (tone)
                {
                    case "":
                        return Tone.Dur;
                    case " ":
                        return Tone.Dur;
                    case "major":
                        return Tone.Dur;
                    case "maj":
                        return Tone.Dur;
                    case "dur":
                        return Tone.Dur;

                    case "m":
                        return Tone.Moll;
                    case "min":
                        return Tone.Moll;
                    case "minor":
                        return Tone.Moll;
                    case "moll":
                        return Tone.Moll;
                    default:
                        throw new ToneNotFoundException();
                }
            }

            public static Entry First(string hashTagString)
            {
                var parser = new EntryParser(hashTagString);
                return parser.Parse();
            }

            public static KeyHashTag ToKeyHashTag(HashTag hashTag)
            {
                if (hashTag is KeyHashTag) return hashTag as KeyHashTag;

                var hashTagEntry = First(hashTag);
                if (hashTagEntry != null)
                    return hashTagEntry.HashTag;

                return null;
            }

            private class EntryParser
            {
                public EntryParser(string hashTagString)
                {
                    startIndex = 0;
                    this.hashTagString = hashTagString.ToLower();
                }

                private int startIndex;

                private readonly string hashTagString;
                private string hashTagEntry;
                private string hashTagEntryCut;
                private string hashTagMetaValue;

                private string noteString;
                private Note note;
                private Tone tone;

                public Entry Parse()
                {
                    Reset();

                    if (!IsValidStartIndex())
                        return null;

                    ExtractHashTagEntryStrings();

                    if (!TryExtractNote())
                        return ParseNextHashTagEntry();

                    if (hashTagEntryCut == noteString)
                        return BuildEntry(Tone.Dur);

                    ChangeNoteIfIsSharp();

                    return hashTagEntryCut.Length == 0
                        ? BuildEntry(Tone.Dur)
                        : TryExtractToneFromHashTagEntryCutAndBuildEntry();
                }

                private void Reset()
                {
                    startIndex = hashTagString.IndexOf(Hash, startIndex, StringComparison.Ordinal);
                    noteString = string.Empty;
                    hashTagMetaValue = string.Empty;
                    hashTagEntry = string.Empty;
                    hashTagEntryCut = string.Empty;
                }

                private bool IsValidStartIndex()
                {
                    return (startIndex > -1) && (startIndex < hashTagString.Length - 1);
                }

                private void ExtractHashTagEntryStrings()
                {
                    ++startIndex;
                    var word = FirstWord(hashTagString.Substring(startIndex));
                    var wordParts = word.Split(Meta[0]);
                    Contract.Assert(wordParts.Length > 0);
                    hashTagEntryCut = hashTagEntry = wordParts[0];
                    
                    if (wordParts.Length == 2)
                        hashTagMetaValue = wordParts[1];

                    noteString = hashTagEntryCut.Substring(0, 1);
                }

                private bool TryExtractNote()
                {
                    try
                    {
                        note = ToNote(noteString);
                        hashTagEntryCut = hashTagEntryCut.Substring(1);
                        return true;
                    }
                    catch (NoteNotFoundException)
                    {
                        return false;
                    }
                }

                private Entry ParseNextHashTagEntry()
                {
                    startIndex += hashTagEntry.Length;
                    return Parse();
                }

                private Entry TryExtractToneFromHashTagEntryCutAndBuildEntry()
                {
                    try
                    {
                        ExtractToneFromHashTagEntryCut();
                        return BuildEntry();
                    }
                    catch (ToneNotFoundException)
                    {
                        return ParseNextHashTagEntry();
                    }
                }

                private void ExtractToneFromHashTagEntryCut()
                {
                    if (hashTagEntryCut[0] == '-')
                        hashTagEntryCut = hashTagEntryCut.Substring(1);

                    if (MatchMinorTone(hashTagEntryCut))
                        tone = Tone.Moll;
                    else if (MatchMajorTone(hashTagEntryCut))
                        tone = Tone.Dur;
                    else
                        throw new ToneNotFoundException();
                }

                private static bool MatchMinorTone(string word)
                {
                    return MinorTones.FirstOrDefault(word.Equals) != null;
                }

                private static bool MatchMajorTone(string word)
                {
                    return MajorTones.FirstOrDefault(word.Equals) != null;
                }

                private Entry BuildEntry(Note newNote, Tone newTone)
                {
                    var hashTag = new KeyHashTag(new Key(newNote, newTone), hashTagMetaValue);
                    return new Entry(hashTag, startIndex, hashTagEntry.Length);
                }

                private Entry BuildEntry(Tone newTone)
                {
                    return BuildEntry(note, newTone);
                }

                private Entry BuildEntry()
                {
                    return BuildEntry(note, tone);
                }

                private void ChangeNoteIfIsSharp()
                {
                    try
                    {
                        var sharpDelimiter = HashString;
                        if (!hashTagEntryCut.StartsWith(sharpDelimiter))
                        {
                            sharpDelimiter = "is";
                            if (!hashTagEntryCut.StartsWith(sharpDelimiter))
                                return;
                        }

                        hashTagEntryCut = hashTagEntryCut.Substring(sharpDelimiter.Length);
                        note = ToNote(noteString + HashString);
                    }
                    catch (NoteNotFoundException)
                    {
                    }
                }

            }
        }
    }
}