using System.Linq;
using System.Windows.Forms;
using MMK.Marking.Representation;
using MMK.MusicCollectionManager.Model;
using MMK.Tools;
using MMK.Wpf;
using MMK.Wpf.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using MessageBox = System.Windows.MessageBox;
using ThreadState = System.Threading.ThreadState;

namespace MMK.MusicCollectionManager.ViewModel
{
    public class MainViewModel : ObservableObject, IDisposable
    {
        public MainViewModel()
        {
            Items = new ObservableCollection<ItemViewModel>();
            Items.CollectionChanged += ItemsChanged;

            AddItemCommand = new Command(AddItemCommandAction, () => !IsProcessingStarted);
            AddItemsFromDropCommand = new Command<DragEventArgs>(AddItemsFromDropCommandAction, () => !IsProcessingStarted);
            RemoveItemCommand = new Command<ItemViewModel>(RemoveItemCommandAction, () => !IsProcessingStarted);
            RemoveItemsCommand = new Command<IList>(RemoveItemsCommandAction, () => !IsProcessingStarted);

            canProcessEvent = new ManualResetEvent(false);
            StartPauseProcessingCommand = new Command(StartPauseProcessingCommandAction);
            CancellProcessingCommand = new Command(CancellProcessingCommandAction);

            ClosingCommand = new Command<CancelEventArgs>(ClosingCommandAction);

            statusState = "Ready";
        }

        private void ItemsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateCanStartProcessing();
        }

        private bool canStartProcessing;
        private bool hasProcessingOperations;
        private bool isProcessingStartedOrIsEmpty = true;

        public ObservableCollection<ItemViewModel> Items { get; private set; }

        #region Add Remove Items
        private void AddItem(string path)
        {
            if (!FileProcessing.IsDirectory(path) && !FileExtensionParser.IsMp3(path)) return;

            if (Items.Any(item => item.Path == path))
                return;

            Items.Add(path);
        }

        private void AddItemCommandAction()
        {
            var folderPath = DialogPicker.PickFolder();
            if (folderPath != null)
                AddItem(folderPath);
        }

        public ICommand AddItemCommand
        {
            get;
            private set;
        }

        private void AddItemsFromDropCommandAction(DragEventArgs e)
        {
            var items = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (items != null)
                foreach (var path in items)
                    AddItem(path);
        }

        public ICommand AddItemsFromDropCommand
        {
            get;
            private set;
        }

        private void RemoveItemCommandAction(ItemViewModel item)
        {
            Items.Remove(item);
        }

        public ICommand RemoveItemCommand { get; private set; }

        private void RemoveItemsCommandAction(IList items)
        {
            var itemsCopy = new ItemViewModel[items.Count];
            items.CopyTo(itemsCopy, 0);
            foreach (ItemViewModel item in itemsCopy)
                Items.Remove(item);
        }
        public ICommand RemoveItemsCommand
        {
            get;
            private set;
        }
        #endregion

        #region Processing
        private bool isProcessingStarted;
        private bool isProcessingPaused;

        private Thread processingThread;
        private readonly ManualResetEvent canProcessEvent;

        /// <summary>
        /// processingThread main proc
        /// </summary>
        private void OnProcessing()
        {
            IsProcessingStarted = true;
            IsProcessingPaused = false;

            var processSuccessedItems = new LinkedList<ItemViewModel>();
            foreach (var item in Items)
                if (canProcessEvent.WaitOne())
                {
                    StatusMessage = item.Path;
                    if (item.Process())
                        processSuccessedItems.AddLast(item);
                    ++TotalProgressValue;
                }

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!IsRemoveAfterProcessing) return;
                
                foreach (var item in processSuccessedItems)
                    Items.Remove(item);
            });

            TotalProgressValue = 0;

            processingThread = null;
            IsProcessingStarted = false;
            IsProcessingPaused = false;
            StatusMessage = string.Empty;
            StatusState = "Done";
        }

        private void StartProcessing()
        {
            if (MustCreateProcessingThread)
                CreateAndStartProcessingThread();
            else
                canProcessEvent.Set();

            IsProcessingStarted = true;
            IsProcessingPaused = false;
            StatusState = "Processing";
        }

        private bool MustCreateProcessingThread
        {
            get { return processingThread == null || processingThread.ThreadState == ThreadState.Stopped; }
        }

        private void CreateAndStartProcessingThread()
        {
            ItemViewModel.ProcessItem += SelectedProcessingOperations;
            processingThread = new Thread(OnProcessing);
            canProcessEvent.Set();
            processingThread.Start();
        }

        private void PauseProcessing()
        {
            if (processingThread != null)
                canProcessEvent.Reset();
            IsProcessingPaused = true;
            StatusState = "Paused";
        }

        private void CancellProcessing()
        {
            if (processingThread != null)
                try
                {
                    processingThread.Abort();
                }
                catch (ThreadStartException) { }
                finally
                {
                    processingThread = null;
                    canProcessEvent.Reset();
                }

            TotalProgressValue = 0;
            IsProcessingStarted = false;
            IsProcessingPaused = false;
            StatusState = "Canceled";
            StatusMessage = "At : " + StatusMessage;
        }

        /// <summary>
        /// This way we start/pause  processing
        /// </summary>
        public bool IsProcessingStarted
        {
            get { return isProcessingStarted; }
            private set
            {
                if (value == isProcessingStarted) return;

                isProcessingStarted = value;
                NotifyPropertyChanged();
                UpdateIsProcessingStartedOrIsEmpty();
            }
        }

        public bool IsProcessingPaused
        {
            get { return isProcessingPaused; }
            private set
            {
                if (value ^ isProcessingPaused) return;

                isProcessingPaused = value;
                NotifyPropertyChanged();
            }
        }

        #region Processing::Commands

        private void CancellProcessingCommandAction()
        {
            if (!isProcessingStarted) return;

            PauseProcessing();
            var result = ShowCancelProcessingMessageBox();
            ProcessCancelProcessingMessageBoxResult(result);
        }

        private void ProcessCancelProcessingMessageBoxResult(MessageBoxResult result)
        {
            switch (result)
            {
                case MessageBoxResult.Yes:
                    CancellProcessing();
                    break;
                case MessageBoxResult.No:
                    StartProcessing();
                    break;
                case MessageBoxResult.None:
                    StartProcessing();
                    break;
            }
        }

        private static MessageBoxResult ShowCancelProcessingMessageBox()
        {
            return MessageBox.Show(
                Application.Current.MainWindow,
                "Do you realy want to cancell processing ?",
                "Warning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );
        }


        public ICommand CancellProcessingCommand
        {
            get;
            private set;
        }

        private void StartPauseProcessingCommandAction()
        {
            if (isProcessingStarted)
            {
                if (isProcessingPaused)
                    StartProcessing();
                else
                    PauseProcessing();
            }
            else
            {
                StartProcessing();
            }
        }

        public ICommand StartPauseProcessingCommand { get; private set; }

        #endregion

        #region Processing::Progress

        private double totalProgressValue;

        public double TotalProgressValue
        {
            get { return totalProgressValue; }
            private set
            {
                if (!(Math.Abs(value - totalProgressValue) > 0.01)) return;

                totalProgressValue = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region Processing Opreations Flags

        private bool isNormalizeTrackName;
        private bool isWriteId3Tags;
        private bool isDeleteOldId3Tags;
        private bool isRemoveAfterProcessing;

        public bool IsNormalizeTrackName
        {
            get { return isNormalizeTrackName; }
            set
            {
                if (value ^ isNormalizeTrackName) return;

                isNormalizeTrackName = value;
                NotifyPropertyChanged();
                UpdateProcessingOperations();
            }
        }
        public bool IsWriteId3Tags
        {
            get { return isWriteId3Tags; }
            set
            {
                if (value ^ isWriteId3Tags) return;

                isWriteId3Tags = value;
                NotifyPropertyChanged();
                UpdateProcessingOperations();
            }
        }
        public bool IsDeleteOldId3Tags
        {
            get { return isDeleteOldId3Tags; }
            set
            {
                if (value ^ isDeleteOldId3Tags) return;

                isDeleteOldId3Tags = value;
                NotifyPropertyChanged();
                UpdateProcessingOperations();
            }
        }
        public bool IsRemoveAfterProcessing
        {
            get { return isRemoveAfterProcessing; }
            set
            {
                if (value ^ isRemoveAfterProcessing) return;

                isRemoveAfterProcessing = value;
                NotifyPropertyChanged();
            }
        }

        private void UpdateProcessingOperations()
        {
            HasProcessingOperations = isNormalizeTrackName || isWriteId3Tags || isDeleteOldId3Tags;
        }

        public bool HasProcessingOperations
        {
            get
            {
                return hasProcessingOperations;
            }
            private set
            {
                if (hasProcessingOperations ^ value) return;

                hasProcessingOperations = value;
                NotifyPropertyChanged();
                UpdateCanStartProcessing();
            }
        }

        public void UpdateCanStartProcessing()
        {
            CanStartProcessing = (Items.Count > 0) && HasProcessingOperations;
            UpdateIsProcessingStartedOrIsEmpty();
        }

        public bool CanStartProcessing
        {
            get { return canStartProcessing; }
            private set
            {
                if (value ^ canStartProcessing) return;

                canStartProcessing = value;
                NotifyPropertyChanged();
            }
        }

        private void UpdateIsProcessingStartedOrIsEmpty()
        {
            IsProcessingStartedOrIsEmpty = (Items.Count == 0) || IsProcessingStarted;
        }
        public bool IsProcessingStartedOrIsEmpty
        {
            get { return isProcessingStartedOrIsEmpty; }
            private set
            {
                if (value ^ isProcessingStartedOrIsEmpty) return;

                isProcessingStartedOrIsEmpty = value;
                NotifyPropertyChanged();
            }
        }

        public FileItemProcessEvent AllProcessingOperations
        {
            get
            {
                var processingOperations = new FileItemProcessEvent(UpdadeStatusMessage);
                processingOperations += NormalizeTrackName;
                processingOperations += TryDeleteId3Tags;
                processingOperations += TryWriteId3Tags;
                return processingOperations;
            }
        }

        public FileItemProcessEvent SelectedProcessingOperations
        {
            get
            {
                var processingOperations = new FileItemProcessEvent(UpdadeStatusMessage);
                if (isNormalizeTrackName) processingOperations += NormalizeTrackName;
                if (isDeleteOldId3Tags) processingOperations += TryDeleteId3Tags;
                if (isWriteId3Tags) processingOperations += TryWriteId3Tags;
                return processingOperations;
            }
        }

        private bool UpdadeStatusMessage(ref string path)
        {
            StatusMessage = path;
            return true;
        }

        private bool NormalizeTrackName(ref string path)
        {
            var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            var folderName = System.IO.Path.GetDirectoryName(path);
            var trackNameModel = TrackNameModel.Parser.Parse(fileName);
            if (folderName != null)
                path = System.IO.Path.Combine(folderName, trackNameModel.FullName + FileProcessing.Mp3Extension);
            return true;
        }

        private bool TryDeleteId3Tags(ref string path)
        {
            return FileProcessing.TryDeleteId3Tags(path);
        }

        private bool TryWriteId3Tags(ref string path)
        {
            return FileProcessing.TryWriteId3Tags(path);
        }

        #endregion

        #region Status Bar
        private string statusState;
        private string statusMessage;

        public string StatusState
        {
            get { return statusState; }
            set
            {
                if (value == statusState) return;

                statusState = value;
                NotifyPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get { return statusMessage; }
            private set
            {
                if (value == statusMessage) return;

                statusMessage = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Behavior
        private void ClosingCommandAction(CancelEventArgs e)
        {
            if (!isProcessingStarted) return;

            PauseProcessing();
            var result = ShowCancelProcessingMessageBox();
            e.Cancel = result == MessageBoxResult.No || result == MessageBoxResult.None;
            ProcessCancelProcessingMessageBoxResult(result);
        }

        public ICommand ClosingCommand
        {
            get;
            private set;
        }

        #endregion

        public void Dispose()
        {
            canProcessEvent.Dispose();
        }
    }
}
