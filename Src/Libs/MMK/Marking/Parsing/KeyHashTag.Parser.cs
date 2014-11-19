using System;
using System.Linq;

namespace MMK.Marking
{
    public partial class KeyHashTag
    {
        public new class Parser : HashTag.Parser
        {

            #region Const
            public const string HashString = "#";
            private static readonly string[] MinorTones = {"moll", "minor", "min"};
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
                public EntryParser(string hashTags)
                {
                    startIndex = 0;
                    this.hashTags = hashTags.ToLower();
                }

                private int startIndex;

                private readonly string hashTags;
                private string hashTagEntry;
                private string hashTagEntryCut;

                private string noteString;
                private Note note;
                private Tone tone;

                public Entry Parse()
                {
                    Reset();

                    if (!IsValidStartIndex()) return null;

                    ExtractHashTagEntryStrings();

                    if (!TryExtractNote())
                        return ParseNextHashTagEntry();

                    if (hashTagEntryCut == noteString)
                        return BuildEntry(Tone.Dur);

                    ChangeNoteIfIsSharp();

                    if (HasHashTagEntryCutNoteWithoutTone())
                        return BuildEntry(Tone.Dur);

                    return TryExtractToneFromHashTagEntryCutAndBuildEntry();
                }

                private void Reset()
                {
                    startIndex = hashTags.IndexOf(Hash, startIndex, StringComparison.Ordinal);

                    noteString = string.Empty;

                    hashTagEntry = string.Empty;
                    hashTagEntryCut = string.Empty;
                }

                private bool IsValidStartIndex()
                {
                    return (startIndex > -1) && (startIndex < hashTags.Length - 1);
                }

                private void ExtractHashTagEntryStrings()
                {
                    ++startIndex;
                    hashTagEntry = hashTags.Substring(startIndex);
                    hashTagEntry = hashTagEntry.Substring(0, CalcHashTagEntryLength());
                    hashTagEntryCut = hashTagEntry;
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

                    if (MinorTones.FirstOrDefault(t => hashTagEntryCut.StartsWith(t)) == null)
                        if (MajorTones.FirstOrDefault(t => hashTagEntryCut.StartsWith(t)) == null)
                            if (hashTagEntryCut.StartsWith("m")) tone = Tone.Moll;
                            else throw new ToneNotFoundException();
                        else tone = Tone.Dur;
                    else tone = Tone.Moll;
                }

                private Entry BuildEntry(Note note, Tone tone)
                {
                    return new Entry(new Key(note, tone), startIndex, hashTagEntry.Length);
                }

                private Entry BuildEntry(Tone tone)
                {
                    return BuildEntry(note, tone);
                }

                private Entry BuildEntry()
                {
                    return BuildEntry(note, tone);
                }


                private int CalcHashTagEntryLength()
                {
                    var hashTagEntryLength = hashTagEntry.IndexOf(' ');

                    return hashTagEntryLength == -1
                        ? hashTagEntry.Length
                        : hashTagEntryLength + 1;
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


                private bool HasHashTagEntryCutNoteWithoutTone()
                {
                    return hashTagEntryCut.Length == 0 || Char.IsWhiteSpace(hashTagEntryCut[0]);
                }
            }
        }
    }
}
