﻿using System;
using System.Diagnostics.Contracts;
using MMK.Utils.Media;

namespace MMK.Processing
{
    public static class KeyColorConverter
    {
        private static ColorHsvModel ToHsvColorModel(Key key)
        {
            var color = new ColorHsvModel {H = 0.85, S = 0.75, V = 1.0};
            const double hStep = ColorHsvModel.HueMax/NoteFactory.NotesCount;

            var keyToneEnumer = (key.IsMoll() ? CircleOfFifths.MinorKeys : CircleOfFifths.MajorKeys).GetEnumerator();

            while (keyToneEnumer.MoveNext())
            {
                if (keyToneEnumer.Current == key)
                    break;
                color.H -= hStep;
            }

            return color;
        }

        public static ColorArgbDoubleModel ToArgbDoubleModel(Key key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            Contract.EndContractBlock();

            return ToHsvColorModel(key).ToArgbDoubleModel();
        }

        public static ColorArgbByteModel ToArgbByteModel(Key key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            Contract.EndContractBlock();

            return ToHsvColorModel(key).ToArgbByteModel();
        }
    }
}