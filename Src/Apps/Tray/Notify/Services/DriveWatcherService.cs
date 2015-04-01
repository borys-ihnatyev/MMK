using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using MMK.KeyDrive.Services;
using MMK.Notify.ViewModels;
using MMK.Notify.Views;
using MMK.Presentation.Windows.Interop;

namespace MMK.Notify.Services
{
    public class DriveWatcherService : DriveDetectorServiceBase
    {
        private readonly KeyDriveWatcherService keyDriveWatcherService;
        private readonly Dictionary<string, Window> driveView;

        public DriveWatcherService(IHwndSource hwndSource, KeyDriveWatcherService keyDriveWatcherService)
            : base(hwndSource)
        {
            this.keyDriveWatcherService = keyDriveWatcherService;
            driveView = new Dictionary<string, Window>();
        }

        protected override void OnDriveConnected(DriveInfo driveInfo)
        {
            var view = new KeyDriveOptionsView(new KeyDriveOptionsViewModel(driveInfo));
            driveView.Add(driveInfo.RootDirectory.FullName, view);
            view.Closed += (s, e) => driveView.Remove(driveInfo.RootDirectory.FullName);
            view.Show();
            view.Activate();
        }

        protected override void OnDriveRemoved(DriveInfo driveInfo)
        {
            TryCloseView(driveInfo);
            keyDriveWatcherService.RemoveWatcher(driveInfo.RootDirectory.FullName);
        }

        private void TryCloseView(DriveInfo driveInfo)
        {
            if (!driveView.ContainsKey(driveInfo.RootDirectory.FullName)) return;
            driveView[driveInfo.RootDirectory.FullName].Close();
            driveView.Remove(driveInfo.RootDirectory.FullName);
        }
    }
}