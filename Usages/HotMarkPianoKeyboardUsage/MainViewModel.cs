using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MMK.HotMark.ViewModel.PianoKeyBoard;
using Sanford.Multimedia.Midi;

namespace MMK.HotMark.PianoKeyboardUsage
{
    public class MainViewModel : Wpf.ViewModel.ViewModel
    {
        public MainViewModel()
        {
            PianoKeyBoard = new PianoKeyBoardViewModel();    
        }


        public PianoKeyBoardViewModel PianoKeyBoard
        {
            get; private set;
        }
    }
}
