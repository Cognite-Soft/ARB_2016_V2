using System;
using System.Configuration;
using System.ServiceProcess;
using Cognite.Arb.Integration.Business;
using Cognite.Arb.Integration.Resource.ExternalDatabase;
using Cognite.Arb.Integration.Resource.WebApi;

namespace Cognite.Arb.Integration.ServiceHost
{
    public partial class WindowsService : ServiceBase
    {
        private readonly IntegratorScheduler _scheduler;

        public WindowsService()
        {
            InitializeComponent();
            var source = new Source();
            var destination = new Destination(ConfigurationManager.AppSettings["ArbServiceBaseAddress"]);
            var log = new Log();
            var integrator = new Integrator(source, destination, log);
            _scheduler = new IntegratorScheduler(integrator);
        }

        protected override void OnStart(string[] args)
        {
            _scheduler.Start();
        }

        protected override void OnStop()
        {
            _scheduler.Stop();
        }

        private class Log : Integrator.ILog
        {
            public void LogException(Exception exception)
            {
            }
        }
    }
}
