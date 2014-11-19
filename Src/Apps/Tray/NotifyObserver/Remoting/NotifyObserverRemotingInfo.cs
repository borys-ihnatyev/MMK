namespace MMK.Notify.Observer.Remoting
{
    public static class NotifyObserverRemotingInfo
    {
        static NotifyObserverRemotingInfo()
        {
            MessageObjectFileName = "MMKNotifyObserver.rem";
            IpcChannelPortName = "MMKN";
            PortUrl = "ipc://" + IpcChannelPortName + "/" + MessageObjectFileName;
        }

        public static readonly string MessageObjectFileName;
        public static readonly string IpcChannelPortName;
        public static readonly string PortUrl;
    }
}
