using System.Collections.Generic;
using MMK.Wpf.ViewModel;

namespace System.Collections.ObjectModel
{
    public class ObservbleInt32 : ObservableObject
    {
        public ObservbleInt32(int number)
        {
            this.number = number;
        }

        private Int32 number;

        public Int32 Number
        {
            get { return number; }
            set
            {
                if (value == number) return;
                number = value;
                NotifyPropertyChanged();
            }
        }

        public static implicit operator int(ObservbleInt32 obj)
        {
            if (ReferenceEquals(obj, null))
                throw new InvalidCastException("cannot cast null");
            return obj.Number;
        }

        public static implicit operator ObservbleInt32(int value)
        {
            return new ObservbleInt32(value);
        }

        public sealed class Comparer : Comparer<ObservbleInt32>
        {
            public override int Compare(ObservbleInt32 x, ObservbleInt32 y)
            {
                if (ReferenceEquals(x, null))
                    return ReferenceEquals(y, null)
                        ? 0
                        : -1;

                return ReferenceEquals(y, null)
                    ? 1
                    : Comparer<Int32>.Default.Compare(x.number, y.number);
            }
        }
    }
}