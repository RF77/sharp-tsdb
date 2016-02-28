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