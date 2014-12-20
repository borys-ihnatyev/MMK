namespace MMK.Wpf.Windows
{
    public partial class Taskbar
    {
        private enum Abm : uint
        {
            GetState = 0x00000004,
            GetTaskbarPos = 0x00000005
        }
    }
}