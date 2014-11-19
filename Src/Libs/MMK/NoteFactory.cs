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
            return (Note) (Math.Abs(value)%NotesCount);
        }
    }
}