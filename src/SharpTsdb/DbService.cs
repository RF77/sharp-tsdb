using System;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;

namespace SharpTsdb
{
    public class DbService
    {
        public string baseAddress = "http://localhost:9003/";
        private IDisposable _server;

        public void Init()
        {
            _server = WebApp.Start<Startup>(url: baseAddress);
        }

        public void Stop()
        {
            _server?.Dispose();
        }
    }

    
}
