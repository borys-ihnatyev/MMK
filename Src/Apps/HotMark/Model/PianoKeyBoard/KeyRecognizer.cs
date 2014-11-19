using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MMK.HotMark.Model.PianoKeyBoard
{
    public class KeyRecognizer
    {
        //order important combination of MinKeyRecognizeNoteCount (MinKeyRecognizeNoteCount!) 
        private const int MaxKeyRecognizeNoteCount = 6;
        private const int MinKeyRecognizeNoteCount = 3;

        private readonly LinkedList<Note> playedNotes;
        private readonly Dictionary<Key,HashSet<Note>> keySignatures;

        public event EventHandler<EventArgs<Key>> KeyRecognized;

        public KeyRecognizer()
        {
            playedNotes = new LinkedList<Note>();
            keySignatures = new Dictionary<Key, HashSet<Note>>();
            InitializeKeyNotes();
        }

        private void InitializeKeyNotes()
        {
            foreach (var key in CircleOfFifths.AllKeys)
                keySignatures.Add(key, new HashSet<Note>(key.Notes));
        }

        public void PlayNote(int midiNote)
        {
            lock (playedNotes)
            {
                if (playedNotes.Count == MaxKeyRecognizeNoteCount)
                    playedNotes.RemoveFirst();

                playedNotes.AddLast(NoteFactory.Create(midiNote));

                TryRecognizeKey();
            }
        }

        private void TryRecognizeKey()
        {
            if (playedNotes.Count < MinKeyRecognizeNoteCount)
                return;

            RecognizeKey();
        }

        private void RecognizeKey()
        {
            var matchedKey = GetFirstMatchedKey(new SortedSet<Note>(playedNotes));

            if(matchedKey != null)
                OnKeyRecognized(matchedKey);
        }

        private Key GetFirstMatchedKey(IEnumerable<Note> historySignature)
        {
            return keySignatures
                .Where(keySignature => keySignature.Value.IsSubsetOf(historySignature))
                .Select(keySignature => keySignature.Key)
                .FirstOrDefault();
        }

        public void Reset()
        {
            lock (playedNotes)
                playedNotes.Clear();
        }

        private void OnKeyRecognized(Key newKey)
        {
            Reset();

            if (KeyRecognized == null)
                return;

            KeyRecognized(this, new EventArgs<Key>(newKey));
        }
    }
}