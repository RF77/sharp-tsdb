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
using System.Runtime.CompilerServices;
using log4net;
using log4net.Core;

namespace SharpTsdb
{
    public class ControllerLogger
    {
        public ControllerLogger(ILog logger)
        {
            Logger = logger;
        }

        public ILog Logger { get; }

        public IDisposable LogDebug(string text = null, [CallerMemberName] string memberName = null)
        {
            return new MethodLogger(text, memberName, Logger, Level.Debug);
        }
    }
}