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
using Mqtt2SharpTsdb;

namespace Mqtt2SharpTsdbService
{
    public partial class Mqtt2SharpTsdbService : ServiceBase
    {
        private readonly string[] _args;
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Mqtt2DbService _service;

        public Mqtt2SharpTsdbService(string[] args)
        {
            _args = args;
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
            Stop();
        }
    }
}