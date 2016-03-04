// /*******************************************************************************
//  * Copyright (c) 2016 by RF77 (https://github.com/RF77)
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the Eclipse Public License v1.0
//  * which accompanies this distribution, and is available at
//  * http://www.eclipse.org/legal/epl-v10.html
//  *
//  * Contributors:
//  *    RF77 - initial API and implementation and/or initial documentation
//  *******************************************************************************/ 

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
            Stop();
        }
    }
}