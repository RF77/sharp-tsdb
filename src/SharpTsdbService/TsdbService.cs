using System.Reflection;
using System.ServiceProcess;
using log4net;
using log4net.Config;
using SharpTsdb;

namespace SharpTsdbService
{
    public partial class TsdbService : ServiceBase
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private DbService _service;

        public TsdbService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            XmlConfigurator.Configure();
            Logger.Info("Starting Sharp TSDB service");
            _service = new DbService();
            _service.Init();
        }

        protected override void OnStop()
        {
            Logger.Info("Stopping Sharp TSDB service");
            _service?.Stop();
            base.Stop();
        }
    }
}
