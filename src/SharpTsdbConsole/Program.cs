using System;
using log4net.Config;
using SharpTsdb;

namespace SharpTsdbConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            var service = new DbService();
            service.Init();

            Console.ReadLine();

            service.Stop();
        }
    }
}
