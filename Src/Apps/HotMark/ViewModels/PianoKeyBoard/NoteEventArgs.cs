using System;

namespace MMK.HotMark.ViewModels.PianoKeyBoard
{
    public class NoteEventArgs : EventArgs
    {
        private readonly int midiNote;

        public NoteEventArgs(Note note, int octave)
        {
            Note = note;
            Octave = octave;
            midiNote = (int)Note + Octave * NoteFactory.NotesCount;
        }

        public Note Note { get; private set; }

        public int Octave { get; private set; }

        public int MidiNote
        {
            get { return midiNote; }
        }
    }
}
