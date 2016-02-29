using System;
using System.Reflection;
using log4net;
using log4net.Config;
using Mqtt2SharpTsdb;

namespace Mqtt2SharpTsdbConsole
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            Logger.Info("Started Mqtt2Sharp Tsdb Console Application");
            var service = new Mqtt2DbService();
            service.Init();

            Console.ReadLine();

            service.Stop();
            Logger.Info("Stopped Mqtt2Sharp Tsdb Console Application");
        }
    }
}
