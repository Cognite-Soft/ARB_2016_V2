using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using Cognite.Arb.Web.ServiceClient;

namespace Cognite.Arb.Schedule.WindowsServiceHost
{
    public partial class WindowsService : ServiceBase
    {
        private readonly object _lock1 = new object();
        private readonly object _lock2 = new object();
        private Thread _thread;
        private ManualResetEvent _event;
        private const int Interval = 30;

        public WindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            lock (_lock2)
            {
                if (_thread != null)
                    OnStop();

                _thread = new Thread(BackgroundThread);
                _event = new ManualResetEvent(false);
                _thread.Start();
            }
        }

        protected override void OnStop()
        {
            lock (_lock1)
            {
                _event.Set();
                if (!_thread.Join(TimeSpan.FromSeconds(10)))
                    _thread.Abort();
                _thread = null;
                _event.Dispose();
                _event = null;
            }
        }

        private void BackgroundThread()
        {
            for (;;)
            {
                var client = new WebApiClient(ConfigurationManager.AppSettings["WebApiAddress"]);
                if (_event.WaitOne(TimeSpan.FromSeconds(Interval)))
                    return;
                client.NotifyCaseWorkersAboutClosingCases("arb1234567890system");
            }
        }
    }
}
