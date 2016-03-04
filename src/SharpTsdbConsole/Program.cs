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

using System;
using System.Reflection;
using log4net;
using log4net.Config;
using SharpTsdb;

namespace SharpTsdbConsole
{
    internal class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static void Main(string[] args)
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