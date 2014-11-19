namespace MMK.HotMark.Model.PianoKeyBoard
{
    public static class MidiNote
    {
        public static int[] MinorChord(Note note, int octave)
        {
            return MinorChord(ToMidiNote(note,octave));
        }

        public static int[] MinorChord(int midiNote)
        {
            return new[] {midiNote, midiNote + 3, midiNote + 7};
        }

        public static int[] MajorChord(Note note, int octave)
        {
            return MajorChord(ToMidiNote(note,octave));
        }

        public static int[] MajorChord(int midiNote)
        {
            return new[] {midiNote, midiNote + 4, midiNote + 7};
        }

        public static int[] Chord(Key key, int octave)
        {
            if (key.IsMoll())
                return MinorChord(key.Note, octave);

            return MajorChord(key.Note, octave);
        }

        public static int ToMidiNote(Note note, int octave)
        {
            return (int) note + octave*NoteFactory.NotesCount;
        }
    }
}