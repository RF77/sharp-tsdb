using System;
using System.Runtime.CompilerServices;
using log4net;
using log4net.Core;

namespace SharpTsdb
{
    public class ControllerLogger
    {
        public ILog Logger { get; }

        public ControllerLogger(ILog logger)
        {
            Logger = logger;
        }

        public IDisposable LogDebug(string text = null, [CallerMemberName]string memberName=null)
        {
            return new MethodLogger(text, memberName, Logger, Level.Debug);
        }
    }
}