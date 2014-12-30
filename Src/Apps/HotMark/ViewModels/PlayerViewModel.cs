using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MMK.HotMark.Model.Files;
using MMK.Wpf.ViewModel;
using IOFile = System.IO.File;


namespace MMK.HotMark.ViewModels
{
    public class PlayerViewModel : ViewModel
    {
        public const double VolumeIncreaseStep = 0.2;
        public readonly double PositionIncreaseStep = TimeSpan.FromSeconds(7).TotalMilliseconds;

        private readonly MediaPlayer player;
        private readonly MediaTimeline timeline;

        private bool isPlaying;
        private double positionMax;
        private TimeSpan duration;

        public PlayerViewModel(string filePath)
        {
            if (!IOFile.Exists(filePath))
                throw new FileNotFoundException("not found", filePath);
            Contract.EndContractBlock();

            File = filePath;

            timeline = new MediaTimeline(new Uri(File.Path, UriKind.Absolute));
            timeline.CurrentTimeInvalidated += (s, e) => OnPositionChanged();

            player = new MediaPlayer();
            player.MediaFailed += OnFileOpenFailed;
            player.MediaOpened += OnFileOpened;
            player.MediaEnded += OnPlaybackEnd;
        }

        private ClockController PlaybackController
        {
            get { return player.Clock.Controller; }
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

                if (value < 0)
                    value = 0;
                else if (value > PositionMax)
                    value = positionMax;

                PlaybackController.Seek(TimeSpan.FromMilliseconds(value), TimeSeekOrigin.BeginTime);

                OnPositionChanged();
            }
        }


        public double Volume
        {
            get { return player.Volume; }
            set
            {
                player.Volume = value;
                NotifyPropertyChanged();
            }
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
                NotifyPropertyChanged("ReminedTime");
                NotifyPropertyChanged();
            }
        }

        public TimeSpan ElapsedTime
        {
            get { return player.Position; }
        }

        public TimeSpan ReminedTime
        {
            get { return Duration - ElapsedTime; }
        }


        private void OnPositionChanged()
        {
            NotifyPropertyChanged("ElapsedTime");
            NotifyPropertyChanged("ReminedTime");
            NotifyPropertyChanged("Position");
        }

        private void OnFileOpenFailed(object sender, ExceptionEventArgs e)
        {
            throw e.ErrorException;
        }

        private void OnFileOpened(object sender, EventArgs eventArgs)
        {
            Contract.Assert(player.NaturalDuration.HasTimeSpan);
            Duration = player.NaturalDuration.TimeSpan;
        }

        private void OnPlaybackEnd(object sender, EventArgs eventArgs)
        {
            IsPlaying = false;
        }

        #region Loading

        protected override void OnLoadData()
        {
            player.Clock = timeline.CreateClock();
        }

        protected override void OnUnloadData()
        {
            PlaybackController.Stop();
            IsPlaying = false;
        }

        #endregion

        #region Commands

// ReSharper disable UnusedAutoPropertyAccessor.Local
        public ICommand PlaybackStateChangeCommand { get; private set; }
        public void PlaybackStateChange()
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

            PlaybackController.Resume();

            IsPlaying = true;
        }

        public void Pause()
        {
            if (!IsPlaying)
                return;

            PlaybackController.Pause();

            IsPlaying = false;
        }

        public ICommand VolumeIncreaseCommand { get; private set; }

        public void VolumeIncrease()
        {
            Volume += VolumeIncreaseStep;
        }


        public ICommand VolumeDecreaseCommand { get; private set; }

        public void VolumeDecrease()
        {
            Volume -= VolumeIncreaseStep;
        }

        public ICommand PositionIncreaseCommand { get; private set; }

        public void PositionIncrease()
        {
            Position += PositionIncreaseStep;
        }


        public ICommand PositionDecreaseCommand { get; private set; }

        public void PositionDecrease()
        {
            Position -= PositionIncreaseStep;
        }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        #endregion

        public event EventHandler FileOpened
        {
            add { player.MediaOpened += value; }
            remove { player.MediaOpened += value; }
        }
    }
}