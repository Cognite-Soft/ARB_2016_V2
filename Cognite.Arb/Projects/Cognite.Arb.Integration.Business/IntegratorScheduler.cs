using System.Threading;

namespace Cognite.Arb.Integration.Business
{
    public class IntegratorScheduler
    {
        private readonly Integrator _integrator;
        private Thread _thread;

        public IntegratorScheduler(Integrator integrator)
        {
            _integrator = integrator;
        }

        public void Start()
        {
            _thread = new Thread(Worker);
            _thread.Start();
        }

        public void Stop()
        {
            _thread.Abort();
            _thread = null;
        }

        private void Worker()
        {
            for (;;)
            {
                Thread.Sleep(10000);
                _integrator.Integrate();
                Thread.Sleep(5000);
            }
        }
    }
}
