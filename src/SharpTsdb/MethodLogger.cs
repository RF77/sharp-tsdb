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
using System.Diagnostics;
using log4net;
using log4net.Core;

namespace SharpTsdb
{
    public class MethodLogger : IDisposable
    {
        private readonly ILog _logger;
        private readonly Level _logLevel;
        private readonly string _methodName;
        private readonly Stopwatch _stopWatch;
        private readonly string _text;

        public MethodLogger(string text, string methodName, ILog logger, Level logLevel)
        {
            _text = text;
            _methodName = methodName;
            _logger = logger;
            _logLevel = logLevel;
            _logger.Logger.Log(typeof (MethodLogger), _logLevel, $"Entered method {_methodName}: {_text}", null);
            _stopWatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopWatch.Stop();
            _logger.Logger.Log(typeof (MethodLogger), _logLevel,
                $"Left method {_methodName} ({_stopWatch.ElapsedMilliseconds}ms)", null);
        }
    }
}