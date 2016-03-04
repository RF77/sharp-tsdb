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
using System.Threading;

namespace FileDb.Impl
{
    public class ReadWritLockable
    {
        private readonly TimeSpan _readWriteTimeOut;
        private readonly ReaderWriterLock _rwl = new ReaderWriterLock();

        public ReadWritLockable(TimeSpan readWriteLockTimeOut)
        {
            _readWriteTimeOut = readWriteLockTimeOut;
        }

        protected void WriterLock(Action action)
        {
            try
            {
                _rwl.AcquireWriterLock(_readWriteTimeOut);
                action();
            }
            finally
            {
                _rwl.ReleaseWriterLock();
            }
        }

        protected T ReaderLock<T>(Func<T> action)
        {
            try
            {
                _rwl.AcquireReaderLock(_readWriteTimeOut);
                return action();
            }
            finally
            {
                _rwl.ReleaseReaderLock();
            }
        }

        protected T WriterLock<T>(Func<T> action)
        {
            try
            {
                _rwl.AcquireWriterLock(_readWriteTimeOut);
                return action();
            }
            finally
            {
                _rwl.ReleaseWriterLock();
            }
        }
    }
}