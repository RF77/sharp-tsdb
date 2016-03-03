using System;
using DbInterfaces.Interfaces;
using FileDb.Impl;
using Microsoft.Owin.Hosting;
using SharpTsdb.Properties;

namespace SharpTsdb
{
    public class DbService
    {
        public static IDbManagement DbManagement { get; } = new DbManagement();
        public string BaseAddress = $"http://10.10.1.77:{Settings.Default.Port}/";
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
