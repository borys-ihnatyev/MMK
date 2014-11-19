using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Windows.Forms;
using MMK.KeyDrive.Models;

namespace MMK.KeyDrive
{
    public partial class MainForm : Form
    {
        private readonly DriveDetector detector;

        public MainForm()
        {
            InitializeComponent();
            Contract.Assert(Handle != null);
            detector = new DriveDetector();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            detector.DriveConnected += DetectorOnDriveConnected;
            detector.DriveRemoved += DetectorOnDriveRemoved;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);            

            if(detector != null)
                detector.WndProc(m.Msg, m.LParam, m.WParam);
        }

        private void DetectorOnDriveRemoved(object sender, EventArgs<DriveInfo> e)
        {
            Log(@"Drive Removed : " + e.Arg);
        }

        private void DetectorOnDriveConnected(object sender, EventArgs<DriveInfo> e)
        {
            Log(@"Drive Connected : " + e.Arg);
        }

        private void Log(string text)
        {
            log.Text += text + Environment.NewLine;
        }
    }
}
