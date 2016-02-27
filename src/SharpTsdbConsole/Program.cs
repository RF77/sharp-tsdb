using System;
using System.Reflection;
using log4net;
using log4net.Config;
using SharpTsdb;

namespace SharpTsdbConsole
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            Logger.Info("Started Sharp Tsdb Console Application");
            var service = new DbService();
            service.Init();

            Console.ReadLine();

            service.Stop();
            Logger.Info("Stopped Sharp Tsdb Console Application");
        }
    }
}
