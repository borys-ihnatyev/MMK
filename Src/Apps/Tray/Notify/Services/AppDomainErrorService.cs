using System;
using System.IO;
using System.Windows;
using MMK.ApplicationServiceModel;

namespace MMK.Notify.Services
{
    public class AppDomainErrorService : IService
    {
        private bool isStarted;
        
        public void Start()
        {
            if(isStarted)
                return;
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += DomainUnhandledException;
#endif

            isStarted = true;
        }

        public void Stop()
        {
            if(!isStarted)
                return;
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException -= DomainUnhandledException;
#endif
            isStarted = false;
        }

        private static void DomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            TryLogException(e.ExceptionObject);
            Application.Current.Shutdown();
        }

        private static void TryLogException(object error)
        {
            if (!(error is Exception))
                return;
            LogException((Exception)error);
        }

        private static void LogException(Exception exception)
        {
            using (var log = new StreamWriter(typeof(App).FullName + ".Error.log"))
            {
                log.WriteLine(exception.Message);
                log.WriteLine(exception.StackTrace);
            }
        }
    }
}