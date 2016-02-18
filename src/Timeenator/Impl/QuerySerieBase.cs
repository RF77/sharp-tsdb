using System;
using System.Collections.Generic;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public abstract class QuerySerieBase<T>:IQuerySerieBase<T> where T : struct
    {
        private string _name;
        private string _originalName;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }


        public string FullName => _originalName == null ? _name : $"{_originalName}.{_name}";

        /// <summary>
        /// Last value before the start time or null
        /// </summary>
        public ISingleDataRow<T> PreviousRow { get; set; }

        IObjectSingleDataRow IObjectQuerySerieBase.NextRow => NextRow;
        public ISingleDataRow<T> LastRow { get; set; }

        public abstract object this[int index] { get; set; }

        IObjectSingleDataRow IObjectQuerySerieBase.LastRow => LastRow;

        IObjectSingleDataRow IObjectQuerySerieBase.PreviousRow => PreviousRow;

        /// <summary>
        /// first value after end time or null
        /// </summary>
        public ISingleDataRow<T> NextRow { get; set; }

        /// <summary>
        /// Name of measurement
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    if (_name != null && _originalName == null)
                    {
                        _originalName = _name;
                    }
                    _name = value;
                }
            }
        }

        public string GroupName { get; set; }
        public string Key { get; set; }

        public string OriginalName => _originalName ?? Name;


        public QuerySerieBase(DateTime? startTime, DateTime? endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        protected QuerySerieBase(IQuerySerieBase<T> serie)
        {
            StartTime = serie.StartTime;
            EndTime = serie.EndTime;
            Name = serie.Name;
            NextRow = serie.NextRow;
            PreviousRow = serie.PreviousRow;
            LastRow = serie.LastRow;
            GroupName = serie.GroupName;
            Key = serie.Key;
        }
    }
}