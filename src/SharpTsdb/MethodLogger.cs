using System;
using System.Diagnostics;
using log4net;
using log4net.Core;

namespace SharpTsdb
{
    public class MethodLogger : IDisposable
    {
        private readonly string _text;
        private readonly string _methodName;
        private readonly ILog _logger;
        private readonly Level _logLevel;
        private readonly Stopwatch _stopWatch;

        public MethodLogger(string text, string methodName, ILog logger, Level logLevel)
        {
            _text = text;
            _methodName = methodName;
            _logger = logger;
            _logLevel = logLevel;
            _logger.Logger.Log(typeof(MethodLogger), _logLevel, $"Entered method {_methodName}: {_text}", null);
            _stopWatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopWatch.Stop();
            _logger.Logger.Log(typeof(MethodLogger), _logLevel, $"Left method {_methodName} ({_stopWatch.ElapsedMilliseconds}ms)", null);
        }
    }
}