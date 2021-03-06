﻿// /*******************************************************************************
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
using DbInterfaces.Interfaces;
using FileDb.Impl;
using Microsoft.Owin.Hosting;
using SharpTsdb.Properties;

namespace SharpTsdb
{
    public class DbService
    {
        private IDisposable _server;

#if DEBUG
        public string BaseAddress = $"http://*:{Settings.Default.Port+1}/";
#else
        public string BaseAddress = $"http://*:{Settings.Default.Port}/";
#endif

        public static IDbManagement DbManagement { get; } = new DbManagement();

        public void Init()
        {
            _server = WebApp.Start<Startup>(BaseAddress);
        }

        public void Stop()
        {
            _server?.Dispose();
        }
    }
}