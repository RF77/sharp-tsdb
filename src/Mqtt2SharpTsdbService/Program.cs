using System.ServiceProcess;

namespace Mqtt2SharpTsdbService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Mqtt2SharpTsdbService()
            };
            HexMaster.Helper.Run(ServicesToRun);
        }
    }
}
