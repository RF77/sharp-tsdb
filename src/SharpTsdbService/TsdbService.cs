using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;

namespace SharpTsdbService
{
    public partial class TsdbService : ServiceBase
    {
        public TsdbService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            XmlConfigurator.Configure();
        }

        protected override void OnStop()
        {
        }
    }
}
