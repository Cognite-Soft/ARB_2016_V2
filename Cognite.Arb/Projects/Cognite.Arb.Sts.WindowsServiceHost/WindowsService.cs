using System.ServiceModel;
using System.ServiceProcess;
using Cognite.Arb.Server.Sts.Service;

namespace Cognite.Arb.Server.Sts.WindowsServiceHost
{
    public partial class WindowsService : ServiceBase
    {
        private ServiceHost _serviceHost = null;

        public WindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            CloseHost();

            _serviceHost = new ServiceHost(typeof(SecurityTokenService));

            _serviceHost.Open();
        }

        protected override void OnStop()
        {
            CloseHost();
        }

        private void CloseHost()
        {
            if (_serviceHost != null)
            {
                _serviceHost.Close();
                _serviceHost = null;
            }
        }
    }
}
