using System;
using System.Collections;
using System.Collections.Generic;

namespace MMK
{
    public class CircleOfFifths
    {
        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Key GetParalel(Key key)
        {
            var note = key.IsMoll() ? (int)key.SubDominant : (int)key.Note - 3;
            var paralelTone = key.IsMoll() ? Tone.Dur : Tone.Moll;
            return new Key(NoteFactory.Create(note), paralelTone);
        }

        public static Key GetNext(Key key)
        {
            const int nextNoteOffset = 7;
            var nextNoteVal = (int) key.Note;
            nextNoteVal += nextNoteOffset;
            var nextNote = NoteFactory.Create(nextNoteVal);
            return new Key(nextNote, key.Tone);
        }

        public static Key GetPrev(Key key)
        {
            const int prevNoteOffset = -7;
            var nextNoteVal = (int) key.Note;
            nextNoteVal += prevNoteOffset;
            var nextNote = NoteFactory.Create(nextNoteVal);
            return new Key(nextNote, key.Tone);
        }

        public static Key GetNextParalel(Key key)
        {
            return GetNext(GetParalel(key));
        }

        public static Key GetPrevParalel(Key key)
        {
            return GetPrev(GetParalel(key));
        }

        public static Key[] GetRelative(Key key)
        {
            const int paralel = 0;
            const int next = 1;
            const int prev = 2;
            const int nextOrPrevToParalel = 3;
            const int nextToParalel = 4;
            const int prevToParalel = 5;
            const int self = 6;

            var relativeKeys = new Key[7];
            relativeKeys[paralel] = GetParalel(key);
            relativeKeys[next] = GetNext(key);
            relativeKeys[prev] = GetPrev(key);

            var nextAnotherToneNote = next;
            if (key.IsDur())
                nextAnotherToneNote = prev;

            relativeKeys[nextOrPrevToParalel] = new Key(relativeKeys[nextAnotherToneNote].Note,
                relativeKeys[paralel].Tone);
            relativeKeys[nextToParalel] = GetNext(relativeKeys[paralel]);
            relativeKeys[prevToParalel] = GetPrev(relativeKeys[paralel]);
            relativeKeys[self] = key;
            return relativeKeys;
        }

        private static IEnumerator<Key> OneToneEnumer(Key initialKey)
        {
            var key = initialKey;
            do
            {
                yield return key;
                key = GetNext(key);
            } while (key != initialKey);
        }

        private static IEnumerator<Key> AllToneEnumer()
        {
            var minKeyEnumer = MinorKeys.GetEnumerator();
            var majKeyEnumer = MajorKeys.GetEnumerator();
            while (minKeyEnumer.MoveNext())
            {
                yield return minKeyEnumer.Current;
                majKeyEnumer.MoveNext();
                yield return majKeyEnumer.Current;
            }
        }

        public class Enumerable : IEnumerable<Key>
        {
            public Enumerable(Func<IEnumerator<Key>> enumer)
            {
                this.enumer = enumer;
            }

            private readonly Func<IEnumerator<Key>> enumer;

            public IEnumerator<Key> GetEnumerator()
            {
                return enumer();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static readonly Enumerable MinorKeys = new Enumerable(() => OneToneEnumer(new Key(Note.A, Tone.Moll)));
        public static readonly Enumerable MajorKeys = new Enumerable(() => OneToneEnumer(new Key(Note.C, Tone.Dur)));
        public static readonly Enumerable AllKeys = new Enumerable(AllToneEnumer);
        private static IEqualityComparer<Key> paralelKeyEqualityComparer;

        public static bool IsEqualOrParalel(Key keyl, Key keyr)
        {
            if (keyl.Tone != keyr.Tone)
                keyl = GetParalel(keyl);
            return keyl.Note == keyr.Note;
        }

        public static IEqualityComparer<Key> ParalelEqualityComparer
        {
            get
            {
                paralelKeyEqualityComparer = paralelKeyEqualityComparer ?? new ParalelKeyEqualityComparer();
                return paralelKeyEqualityComparer;
            }
        }

        private class ParalelKeyEqualityComparer : IEqualityComparer<Key>
        {
            public bool Equals(Key x, Key y)
            {
                return IsEqualOrParalel(x, y);
            }

            public int GetHashCode(Key obj)
            {
                return obj.IsMoll() ? obj.GetHashCode() : GetParalel(obj).GetHashCode();
            }
        }
    }
}