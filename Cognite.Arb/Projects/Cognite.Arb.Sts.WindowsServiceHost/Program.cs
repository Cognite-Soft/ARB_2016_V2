using System.ServiceProcess;

namespace Cognite.Arb.Server.Sts.WindowsServiceHost
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