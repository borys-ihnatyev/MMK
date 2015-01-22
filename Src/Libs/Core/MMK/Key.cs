using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace MMK
{
    [Serializable]
    public sealed class Key : ISerializable
    {
        public Key(Note note, Tone tone)
        {
            Note = note;
            Tone = tone;
        }

        public Key(Key key)
        {
            Note = key.Note;
            Tone = key.Tone;
        }

        [Pure]
        public Note Note { get; private set; }
        
        [Pure]
        public Tone Tone { get; private set; }

        [Pure]
        public Note[] Notes
        {
            get { return new[] {Note, SubDominant, Dominant}; }
        }

        [Pure]
        public Note SubDominant
        {
            get { return NoteFactory.Create((int) Note + (IsMoll() ? 3 : 4)); }
        }

        public bool IsDur()
        {
            return Tone == Tone.Dur;
        }

        public bool IsMoll()
        {
            return Tone == Tone.Moll;
        }

        public bool IsClean()
        {
            if ((int) Note < 5)
                return (int) Note%2 == 0;

            return (int) Note%2 == 1;
        }

        public bool IsSharpness()
        {
            return !IsClean();
        }

        [Pure]
        public Note Dominant
        {
            get { return NoteFactory.Create((int) Note + 7); }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Key && Equals((Key) obj);
        }

        private bool Equals(Key other)
        {
            if (ReferenceEquals(other,null))
                return false;
            return Note == other.Note && Tone == other.Tone;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Note*397) ^ (int) Tone;
            }
        }


        public override string ToString()
        {
            return ToString(KeyNotation.Default);
        }

        public string ToString(KeyNotation keyNotation)
        {
            switch (keyNotation)
            {
                case KeyNotation.Is_M:
                    return (Note + ((Tone == Tone.Dur) ? "" : "m")).ToLower();
                case KeyNotation.IsMollDur:
                    return (Note + Tone.ToString()).ToLower();
                case KeyNotation.IsMinMaj:
                    return (Note + ((Tone == Tone.Dur) ? "maj" : "min")).ToLower();
                case KeyNotation.IsMinorMajor:
                    return (Note + ((Tone == Tone.Dur) ? "major" : "minor")).ToLower();
                case KeyNotation.Sharp_M:
                    return (Note.ToString().Replace("is", "#") + ((Tone == Tone.Dur) ? "" : "m")).ToLower();
                case KeyNotation.SharpMollDur:
                    return (Note.ToString().Replace("is", "#") + Tone).ToLower();
                case KeyNotation.SharpMinMaj:
                    return (Note.ToString().Replace("is", "#") + ((Tone == Tone.Dur) ? "maj" : "min")).ToLower();
                case KeyNotation.SharpMajorMinor:
                    return (Note.ToString().Replace("is", "#") + ((Tone == Tone.Dur) ? "major" : "minor")).ToLower();
                case KeyNotation.IsStrip_M:
                    return (Note + ((Tone == Tone.Dur) ? "" : "-m")).ToLower();
                case KeyNotation.IsStripMollDur:
                    return (Note + "-" + Tone).ToLower();
                case KeyNotation.IsStripMinMaj:
                    return (Note + ((Tone == Tone.Dur) ? "-maj" : "-min")).ToLower();
                case KeyNotation.IsStripMinorMajor:
                    return (Note + ((Tone == Tone.Dur) ? "-major" : "-minor")).ToLower();
                case KeyNotation.Sharp_StripM:
                    return (Note.ToString().Replace("is", "#") + ((Tone == Tone.Dur) ? "" : "-m")).ToLower();
                case KeyNotation.SharpStripMollDur:
                    return (Note.ToString().Replace("is", "#") + ((Tone == Tone.Dur) ? "-dur" : "-moll")).ToLower();
                case KeyNotation.SharpStripMinMaj:
                    return (Note.ToString().Replace("is", "#") + ((Tone == Tone.Dur) ? "-maj" : "-min")).ToLower();
                case KeyNotation.SharpStripMajorMinor:
                    return (Note.ToString().Replace("is", "#") + ((Tone == Tone.Dur) ? "-major" : "-minor")).ToLower();
                case KeyNotation.CamelotWithoutTone:
                    return CamelotWithoutNotation();
                default:
                    //impossible
                    throw new InvalidOperationException();
            }
        }

        private string CamelotWithoutNotation()
        {
            var toneCircle = IsMoll() ? CircleOfFifths.MinorKeys : CircleOfFifths.MajorKeys;

            var keyOrder = 0;

            foreach (var key in toneCircle)
                if (key == this)
                    break;
                else
                    ++keyOrder;

            return String.Format("{0:D2}", keyOrder);
        }


        public static bool operator ==(Key keyLeft, Key keyRight)
        {
            if (!ReferenceEquals(keyLeft, null))
                return keyLeft.Equals(keyRight);
            return ReferenceEquals(keyRight, null);
        }

        public static bool operator !=(Key keyLeft, Key keyRight)
        {
            return !(keyLeft == keyRight);
        }

        #region Serialization

        private Key(SerializationInfo info, StreamingContext context)
        {
            Note = (Note) info.GetValue("Note", typeof (Note));
            Tone = (Tone) info.GetValue("Tone", typeof (Tone));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException("info");
            info.AddValue("Note", Note);
            info.AddValue("Tone", Tone);
        }

        #endregion
    }
}