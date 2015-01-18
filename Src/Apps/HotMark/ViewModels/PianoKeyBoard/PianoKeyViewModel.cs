using System;
using System.Windows.Input;
using System.Windows.Media;
using MMK.Presentation.Tools;
using MMK.Presentation.ViewModel;

namespace MMK.HotMark.ViewModels.PianoKeyBoard
{
    public class PianoKeyViewModel : ViewModel
    {
        private bool isPressed;

        private readonly Key key;
        private Brush pressedColor;

        public event EventHandler<NoteEventArgs> Pressed;
        public event EventHandler<NoteEventArgs> Released;

        public PianoKeyViewModel(Note note, int octave)
        {
            isPressed = false;

            Note = note;
            Octave = octave;
            MidiNote = (int)note + octave*NoteFactory.NotesCount;

            key = new Key(note, Tone.Dur);

            pressedColor = Brushes.Orange;

            PressCommand = new Command(Press);
            ReleaseCommand = new Command(Release);
            MouseEnterCommand = new Command<MouseEventArgs>(MouseEnter);
            MouseLeaveCommand = new Command(MouseLeave);
        }

        public bool IsPressed
        {
            get { return isPressed; }
            set
            {
                isPressed = value;
                NotifyPropertyChanged();
            }
        }

        public Note Note { get; private set; }
        public int Octave { get; private set; }

        public int MidiNote { get; private set; }

        public bool IsSharpness()
        {
            return key.IsSharpness();
        }

        public Brush Color
        {
            get { return IsSharpness() ? Brushes.Black : Brushes.White; }
        }

        public Brush PressedColor
        {
            get { return pressedColor; }
            set
            {
                if (Equals(value, pressedColor))
                    return;
                pressedColor = value;
                NotifyPropertyChanged();
            }
        }

        #region Commands

        public ICommand PressCommand { get; private set; }

        public void Press()
        {
            if (IsPressed) return;

            OnPress();

            IsPressed = true;
        }

        private void OnPress()
        {
            if (Pressed != null)
                Pressed(this, new NoteEventArgs(Note, Octave));
        }


        public ICommand ReleaseCommand { get; private set; }

        public void Release()
        {
            if (!IsPressed) return;

            OnRelease();

            IsPressed = false;
        }

        private void OnRelease()
        {
            if (Released != null)
                Released(this, new NoteEventArgs(Note, Octave));
        }


        public ICommand MouseEnterCommand { get; private set; }

        private void MouseEnter(MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                Press();
        }
        

        public ICommand MouseLeaveCommand { get; private set; }
        
        private void MouseLeave()
        {
            if(IsPressed)
                Release();
        }

        #endregion
    }
}
