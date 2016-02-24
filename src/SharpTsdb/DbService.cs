﻿using System;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using Microsoft.Owin.Hosting;
using SharpTsdb.Properties;

namespace SharpTsdb
{
    public class DbService
    {
        public static IDbManagement DbManagement { get; } = new DbManagement();
        public string BaseAddress = $"http://localhost:{Settings.Default.Port}/";
        private IDisposable _server;

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
