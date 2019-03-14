using Microsoft.Owin.Hosting;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;

namespace _4ib.NetworkInterfaceManager
{
    public class NetworkInterfaceService : ServiceBase
    {
        private IDisposable _server = null;

        public static readonly string Name = "4ib.NetworkInterfaceManager";

        public Action<string> LoggingInfo { get; set; }
        public Action<string> LoggingError { get; set; }

        public string BaseAddress { get; set; }

        public void Start()
        {
            try
            {
                EventLog.Source = Name;

                string portSetting = ConfigurationManager.AppSettings["port"];

                if (int.TryParse(portSetting, out int port))
                {
                    BaseAddress = $"http://localhost:{port}/";
                }
                else
                {
                    LoggingInfo?.Invoke($"Port is not configured or incorrect value '{portSetting}'");
                    return;
                }

                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

                _server = WebApp.Start<Startup>(url: BaseAddress);

                LoggingInfo?.Invoke($"Listen at: {BaseAddress}");
            }
            catch(Exception exc)
            {
                LoggingError?.Invoke(exc.ToString());
            }
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        protected override void OnStop()
        {
            try
            {
                _server?.Dispose();
            }
            catch (Exception e)
            {
                LoggingError?.Invoke(e.ToString());
            }

            base.OnStop();
        }

        private void OnUnhandledException(object obj, UnhandledExceptionEventArgs e)
        {
            LoggingError?.Invoke(e?.ExceptionObject?.ToString());
        }
    }
}
