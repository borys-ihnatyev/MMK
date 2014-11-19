using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace MMK.HotMark.Model.PianoKeyBoard
{
    using Key = System.Windows.Input.Key; 

    public class PianoKeyBoardMidiMapper
    {
        private const string Map = "zsxdcvgbhnjm,l.;/q2w3e4r";

        private readonly HashSet<int> pressedMidiNotes; 

        private int firstOctave;

        public PianoKeyBoardMidiMapper(int firstOctave)
        {
            FirstOctave = firstOctave;
            pressedMidiNotes = new HashSet<int>();
        }

        public int FirstOctave
        {
            get
            {
                return firstOctave;
            }
            set
            {
                if(value < 0)
                    throw new ArgumentException(@"octave must be grater then -1","value");
                Contract.EndContractBlock();

                firstOctave = value;
            }
        }

        public int[] Press(Key key)
        {
            var notes = GetChordNotes(key);
            if (notes.Count > 0)
                return notes.ToArray();

            var midiNote = MidiNote(key);
       
            if (midiNote == -1)
                return new int[0];

            pressedMidiNotes.Add(midiNote);

            return GetNotes(midiNote);
        }

        private static int[] GetNotes(int midiNote)
        {
            if (IsPlayMinorChord)
                return PianoKeyBoard.MidiNote.MinorChord(midiNote);
            if (IsPlayMajorChord)
                return PianoKeyBoard.MidiNote.MajorChord(midiNote);
            return new[] {midiNote};
        }

        public int[] Release(Key key)
        {
            var notes = GetChordNotes(key);

            if (notes.Count > 0)
            {
                pressedMidiNotes.ForEach(note => notes.Remove(note));
                return notes.ToArray();
            }

            var midiNote = MidiNote(key);

            if (midiNote == -1)
                return new int[0];

            pressedMidiNotes.Remove(midiNote);

            return GetNotes(midiNote);
        }

        private List<int> GetChordNotes(Key key)
        {
            var notes = new List<int>(pressedMidiNotes.Count*2);

            if (IsMinorChordKey(key))
                pressedMidiNotes.ForEach(note => notes.AddRange(PianoKeyBoard.MidiNote.MinorChord(note)));

            if (IsMajorChordKey(key))
                pressedMidiNotes.ForEach(note => notes.AddRange(PianoKeyBoard.MidiNote.MajorChord(note)));

            return notes;
        }

        private static bool IsMinorChordKey(Key key)
        {
            return key == Key.LeftCtrl || key == Key.RightCtrl;

        }

        private static bool IsMajorChordKey(Key key)
        {
            return key == Key.LeftShift || key == Key.RightShift;
        }

        private int MidiNote(Key key)
        {
            for (var i = 0; i < Map.Length; i++)
                if (CharToKey(Map[i]) == key)
                    return i + firstOctave * NoteFactory.NotesCount;
            return -1;
        }

        private static bool IsPlayMinorChord
        {
            get { return Keyboard.Modifiers == ModifierKeys.Control; }
        }

        private static bool IsPlayMajorChord
        {
            get { return Keyboard.Modifiers == ModifierKeys.Shift; }
        }

        private static Key CharToKey(char character)
        {
            return KeyInterop.KeyFromVirtualKey(Native.VkKeyScan(character));
        }

        private static class Native
        {
            [DllImport("user32.dll")]
            public static extern short VkKeyScan(char charcter);
        }
    }
}
