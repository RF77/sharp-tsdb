using System.ServiceProcess;
using log4net.Config;
using SharpTsdb;

namespace SharpTsdbService
{
    public partial class TsdbService : ServiceBase
    {
        private DbService _service;

        public TsdbService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            XmlConfigurator.Configure();
            _service = new DbService();
            _service.Init();
        }

        protected override void OnStop()
        {
            _service?.Stop();
            base.Stop();
        }
    }
}
