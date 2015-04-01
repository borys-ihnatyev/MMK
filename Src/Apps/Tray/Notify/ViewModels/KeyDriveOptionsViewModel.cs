using System.IO;
using System.Windows.Input;
using MMK.ApplicationServiceModel;
using MMK.KeyDrive.Services;
using MMK.Presentation.ViewModel;

namespace MMK.Notify.ViewModels
{
    public sealed class KeyDriveOptionsViewModel : ViewModel
    {
        private readonly DriveInfo driveInfo;
        private readonly KeyDriveWatcherService keyDriveWatcherService;

        public KeyDriveOptionsViewModel(DriveInfo driveInfo)
        {
            this.driveInfo = driveInfo;
            keyDriveWatcherService = IoC.Get<KeyDriveWatcherService>();
        }

        public DriveInfo DriveInfo
        {
            get { return driveInfo; }
        }
            
        public ICommand AddWatcherCommand { get; set; }
        private void AddWatcher()
        {
            keyDriveWatcherService.AddWatcher(driveInfo.RootDirectory.FullName, driveInfo.RootDirectory.FullName);    
        }
    }
}