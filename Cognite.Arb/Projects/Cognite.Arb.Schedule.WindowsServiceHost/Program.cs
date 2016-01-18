using System.ServiceProcess;

namespace Cognite.Arb.Schedule.WindowsServiceHost
{
    static class Program
    {
        static void Main()
        {
            var servicesToRun = new ServiceBase[] { new WindowsService() };
            ServiceBase.Run(servicesToRun);
        }
    }
}
