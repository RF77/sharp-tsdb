using System;
using System.Collections.Generic;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class Measurement : IMeasurement
    {
        public void AddDataPoints(IEnumerable<IDataRow> row)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDataRow> GetDataPoints(DateTime? @from, DateTime? to)
        {
            throw new NotImplementedException();
        }
    }
}