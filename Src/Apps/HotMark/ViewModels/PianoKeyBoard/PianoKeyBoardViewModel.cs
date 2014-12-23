using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MMK.HotMark.Model.PianoKeyBoard;
using MMK.HotMark.PianoKeyboardUsage;
using MMK.Wpf;
using Sanford.Multimedia.Midi;

namespace MMK.HotMark.ViewModels.PianoKeyBoard
{
    public class PianoKeyBoardViewModel : Wpf.ViewModel.ViewModel
    {
        private readonly int firstOctave;

        private readonly ObservableCollection<PianoKeyViewModel> keyViewModels;
        private readonly OutputDevice outMidiDevice;
        private readonly PianoKeyBoardMidiMapper mapper;
        private readonly KeyRecognizer keyRecognizer;
        private Key recognizedKey;
        private MidiInstrument currentMidiInstrument;


        public PianoKeyBoardViewModel()
        {
            firstOctave = 4;

            keyViewModels = new ObservableCollection<PianoKeyViewModel>();
            Keys = new ReadOnlyObservableCollection<PianoKeyViewModel>(keyViewModels);

            outMidiDevice = new OutputDevice(0);
            mapper = new PianoKeyBoardMidiMapper(firstOctave);
            keyRecognizer = new KeyRecognizer();
            keyRecognizer.KeyRecognized += KeyRecognizerOnKeyRecognized;

            var midiInstrumentValues = (MidiInstrument[])Enum.GetValues(typeof (MidiInstrument));
            MidiInstruments = new ObservableCollection<MidiInstrument>(midiInstrumentValues);

            KeyDownCommand = new Command<KeyEventArgs>(KeyDown);
            KeyUpCommand = new Command<KeyEventArgs>(KeyUp);
        }

        private void KeyRecognizerOnKeyRecognized(object sender, EventArgs<Key> eventArgs)
        {
            RecognizedKey = eventArgs.Arg;
            var recognizedKeyBrush = KeyColorConverter.Convert(RecognizedKey);
            keyViewModels.ForEach(vm => vm.PressedColor = recognizedKeyBrush);
        }

        public Key RecognizedKey
        {
            get { return recognizedKey; }
            private set
            {
                if (value == recognizedKey) return;
                recognizedKey = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<MidiInstrument> MidiInstruments
        {
            get;
            private set;
        }

        public MidiInstrument CurrentMidiInstrument
        {
            get { return currentMidiInstrument; }
            set
            {
                if (value == currentMidiInstrument)
                    return;

                currentMidiInstrument = value;
                OnCurrentMidiInstrumentChanged();
                NotifyPropertyChanged();
            }
        }

        private void OnCurrentMidiInstrumentChanged()
        {
            outMidiDevice.Send(new ChannelMessage(ChannelCommand.ProgramChange, 0, (int)currentMidiInstrument));
        }

        public ReadOnlyObservableCollection<PianoKeyViewModel> Keys { get; private set; }

        public int FirstOctave
        {
            get { return firstOctave; }
        }


        protected override void OnLoadData()
        {
            PushOctave(firstOctave);
            PushOctave(firstOctave + 1);

            Keys = new ReadOnlyObservableCollection<PianoKeyViewModel>(keyViewModels);
        }

        private void PushOctave(int octave)
        {
            Enumerable
                .Range(0,NoteFactory.NotesCount)
                .Select(note => CreatePianoKey(NoteFactory.Create(note), octave))
                .ForEach(keyViewModels.Add);
        }

        private PianoKeyViewModel CreatePianoKey(Note note, int octave)
        {
            var keyViewModel = new PianoKeyViewModel(note, octave);
            keyViewModel.Pressed += OnPianoKeyPressed;
            keyViewModel.Released += OnPianoKeyReleased;
            return keyViewModel;
        }


        private void OnPianoKeyPressed(object sender, NoteEventArgs e)
        {
            NoteOn(e.MidiNote);
        }

        private void NoteOn(int note)
        {
            outMidiDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, note, 127));
            keyRecognizer.PlayNote(note);
        }


        private void OnPianoKeyReleased(object sender, NoteEventArgs e)
        {
            NoteOff(e.MidiNote);
        }

        private void NoteOff(int note)
        {
            outMidiDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, note, 0));
        }

        #region Commands

        public ICommand KeyDownCommand { get; private set; }

        private void KeyDown(KeyEventArgs e)
        {
            if (e.IsRepeat) return;

            var midiNotes = mapper.Press(e.Key);
            midiNotes.ForEach(NoteOn);
            
            GetKeyViewModels(midiNotes).ForEach(key => key.IsPressed = true);
        }


        public ICommand KeyUpCommand { get; private set; }

        private void KeyUp(KeyEventArgs e)
        {
            if (e.IsRepeat) return;
            
            var midiNotes = mapper.Release(e.Key);
            midiNotes.ForEach(NoteOff);

            GetKeyViewModels(midiNotes).ForEach(key => key.IsPressed = false);
        }

        private IEnumerable<PianoKeyViewModel> GetKeyViewModels(IEnumerable<int> midiNotes)
        {
            return midiNotes
                .Select(midiNote => keyViewModels.FirstOrDefault(key => key.MidiNote == midiNote))
                .Where(key => key != null);
        }

        #endregion

        protected override void OnUnloadData()
        {
            outMidiDevice.Dispose();
        }
    }
}