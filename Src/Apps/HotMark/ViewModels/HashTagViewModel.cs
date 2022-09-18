﻿using System;
using MMK.Marking;
using MMK.Presentation.ViewModel;

namespace MMK.HotMark.ViewModels
{
    public partial class HashTagViewModel : ObservableObject
    {
        private HashTag hashTag;
        private string hashTagValue;
        private bool isSelected;

        public HashTagViewModel(string hashTagValue)
        {
            HashTagValue = hashTagValue;
        }

        public HashTagViewModel()
            : this(string.Empty)
        {
        }

        public bool IsSelected
        {
            get { return isSelected; }
            internal set
            {
                if (isSelected == value) return;

                isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public string HashTagValue
        {
            get { return hashTagValue; }
            set
            {
                value = value.Trim().ToLower();
                if (value == hashTagValue) return;

                hashTagValue = value;

                var hashTagEntry = HashTag.Parser.First(HashTag.Hash + hashTagValue);
                HashTag = hashTagEntry == null ? new HashTag() : hashTagEntry.HashTag;

                NotifyPropertyChanged();
            }
        }

        public HashTag HashTag
        {
            get { return hashTag; }
            private set
            {
                if (Equals(value, hashTag)) return;

                hashTag = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEmpty
        {
            get { return String.IsNullOrWhiteSpace(HashTagValue); }
        }

        public override string ToString()
        {
            return HashTag.ToString();
        }
    }
}