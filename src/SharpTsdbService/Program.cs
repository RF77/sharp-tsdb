using System.ServiceProcess;

namespace SharpTsdbService
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
                new TsdbService()
            };
            HexMaster.Helper.Run(ServicesToRun);
        }
    }
}
