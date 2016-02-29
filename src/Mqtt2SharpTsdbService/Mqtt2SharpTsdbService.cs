using System.Reflection;
using System.ServiceProcess;
using log4net;
using log4net.Config;
using Mqtt2SharpTsdb;

namespace Mqtt2SharpTsdbService
{
    public partial class Mqtt2SharpTsdbService : ServiceBase
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Mqtt2DbService _service;

        public Mqtt2SharpTsdbService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            XmlConfigurator.Configure();
            Logger.Info("Starting Mqtt to Sharp TSDB service");
            _service = new Mqtt2DbService();
            _service.Init();
        }

        protected override void OnStop()
        {
            Logger.Info("Stopping Mqtt to Sharp TSDB service");
            _service?.Stop();
            base.Stop();
        }
    }
}
