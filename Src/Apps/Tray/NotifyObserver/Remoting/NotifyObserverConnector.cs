using System;
using System.Runtime.Remoting;

namespace MMK.Notify.Observer.Remoting
{
    public class NotifyObserverConnector
    {
        private readonly object application;

        public NotifyObserverConnector(object application)
        {
            this.application = application;
            NotifyObserver = null;
        }

        public bool IsConnected
        {
            get
            {
                if (NotifyObserver == null) return false;

                try
                {
                    NotifyObserver.TestConnection();
                    return true;
                }
                catch (RemotingException)
                {
                    return false;
                }
            }
        }

        public INotifyObserver NotifyObserver
        {
            get;
            private set;
        }

        public bool TryConnect() 
        {   
            try
            {
                Connect();
                return true;
            }
            catch (RemotingException)
            {
                return false;
            }
        }

        public void Connect()
        {
            if(IsConnected) return;
            if(application is INotifyObserverOwner)
                ConnectDirect();
            else
                ConnectRemote();
        }

        private void ConnectDirect()
        {
            NotifyObserver = ((INotifyObserverOwner) application).NotifyObserver;
        }

        private void ConnectRemote()
        {
            NotifyObserver = (INotifyObserver) Activator.GetObject(typeof (INotifyObserver), NotifyObserverRemotingInfo.PortUrl);
            NotifyObserver.TestConnection();
        }
    }
}
