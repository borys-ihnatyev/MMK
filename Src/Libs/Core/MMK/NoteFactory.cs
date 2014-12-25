using System;
using System.Diagnostics.Contracts;

namespace MMK
{
    public class NoteFactory
    {
        public const int NotesCount = 12;

        [Pure]
        public static Note Create(int value)
        {
            value = value%NotesCount;

            if (value < 0)
                value = NotesCount + value;

            return (Note) (value%NotesCount);
        }
    }
}