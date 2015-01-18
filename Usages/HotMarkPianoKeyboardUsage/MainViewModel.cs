using MMK.HotMark.ViewModels.PianoKeyBoard;
using MMK.Presentation.ViewModel;

namespace MMK.HotMark.PianoKeyboardUsage
{
    public class MainViewModel : ViewModel
    {
        public MainViewModel()
        {
            PianoKeyBoard = new PianoKeyBoardViewModel();
        }


        public PianoKeyBoardViewModel PianoKeyBoard { get; private set; }
    }
}