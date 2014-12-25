using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MMK.HotMark.Model.Files;
using MMK.Wpf;
using MMK.Wpf.ViewModel;
using IOFile = System.IO.File;


namespace MMK.HotMark.ViewModels
{
    public class PlayerViewModel : ViewModel
    {
        private readonly MediaPlayer player;
        private readonly DispatcherTimer playClock;

        private bool isPlaying;
        private double positionMax;
        private TimeSpan duration;

        public PlayerViewModel(string filePath)
        {
            if (!IOFile.Exists(filePath))
                throw new FileNotFoundException("not found", filePath);
            Contract.EndContractBlock();

            File = filePath;

            playClock = new DispatcherTimer {Interval = TimeSpan.FromSeconds(0.5), IsEnabled = false};
            playClock.Tick += (s, e) => OnPositionChanged();

            player = new MediaPlayer();
            player.MediaFailed += FileOpenFailed;
            player.MediaOpened += FileOpened;
            player.MediaEnded += OnPlaybackEnd;

            var uri = new Uri(File.Path);
            player.Open(uri);
            PlaybackStateChangeCommand = new Command(PlaybackStateChange);
        }

        public FileHashTagModel File { get; private set; }

        public bool IsPlaying
        {
            get { return isPlaying; }
            private set
            {
                if (value == isPlaying)
                    return;
                isPlaying = value;
                NotifyPropertyChanged();
            }
        }

        public double PositionMax
        {
            get { return positionMax; }
            private set
            {
                positionMax = value;
                NotifyPropertyChanged();
            }
        }

        public double Position
        {
            get { return player.Position.TotalMilliseconds; }
            set
            {
                if (!IsDataLoaded)
                    return;

                CheckPosition(value);

                player.Position = TimeSpan.FromMilliseconds(value);

                OnPositionChanged();
            }
        }

        [ContractInvariantMethod]
        private void CheckPosition(double value)
        {
            if (value < 0)
                throw new ArgumentException(
                    String.Format(@"value must be >= 0 but was : {0}", value),
                    "value");
            if (value > PositionMax)
                throw new ArgumentException(
                    String.Format(@"value must be <= {0} but was : {1}", PositionMax, value),
                    "value");
            Contract.EndContractBlock();
        }

        private void OnPositionChanged()
        {
            ElapsedTime = ReminedTime = TimeSpan.Zero;
            NotifyPropertyChanged("Position");
        }


        public TimeSpan Duration
        {
            get { return duration; }
            private set
            {
                if (value == duration)
                    return;

                duration = value;

                PositionMax = duration.TotalMilliseconds;
                ReminedTime = TimeSpan.Zero;

                NotifyPropertyChanged();
            }
        }

        public TimeSpan ElapsedTime
        {
            get { return player.Position; }
            private set { NotifyPropertyChanged(); }
        }

        public TimeSpan ReminedTime
        {
            get { return Duration - ElapsedTime; }
            private set { NotifyPropertyChanged(); }
        }


        public double Volume
        {
            get { return player.Volume; }
            set
            {
                CheckVolume(value);

                player.Volume = value;
                NotifyPropertyChanged();
            }
        }

        [ContractInvariantMethod]
        private void CheckVolume(double value)
        {
            if (value < 0)
                throw new ArgumentException(@"value must be >= 0", "value");
            if (value > 1)
                throw new ArgumentException(@"value must be <= 1", "value");
            Contract.EndContractBlock();
        }

        private void FileOpenFailed(object sender, ExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FileOpened(object sender, EventArgs eventArgs)
        {
            Contract.Assert(player.NaturalDuration.HasTimeSpan);
            Duration = player.NaturalDuration.TimeSpan;
        }

        protected override void OnUnloadData()
        {
            Pause();
            player.Close();
        }

        private void OnPlaybackEnd(object sender, EventArgs eventArgs)
        {
            player.Stop();
            playClock.Stop();
            IsPlaying = false;
        }


        public ICommand PlaybackStateChangeCommand { get; private set; }

        private void PlaybackStateChange()
        {
            if (IsPlaying)
                Pause();
            else
                Play();
        }


        public void Play()
        {
            if (IsPlaying)
                return;

            player.Play();
            playClock.Start();

            IsPlaying = true;
        }

        public void Pause()
        {
            if (!IsPlaying)
                return;

            player.Pause();
            playClock.Stop();

            IsPlaying = false;
        }
    }
}