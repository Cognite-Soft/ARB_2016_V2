using System.ServiceProcess;

namespace Cognite.Arb.Integration.ServiceHost
{
    internal static class Program
    {
        private static void Main()
        {
            var servicesToRun = new ServiceBase[] {new WindowsService()};
            ServiceBase.Run(servicesToRun);
        }
    }
}