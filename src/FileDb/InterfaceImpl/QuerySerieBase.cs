using System;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class QuerySerieBase<T>:IQuerySerieBase<T> where T : struct
    {
        private string _name;
        private string _originalName;
        public DateTime? StartTime { get; }
        public DateTime? StopTime { get; }


        public string FullName => _originalName == null ? _name : $"{_originalName}.{_name}";

        /// <summary>
        /// Last value before the start time or null
        /// </summary>
        public ISingleDataRow<T> PreviousRow { get; set; }

        IObjectSingleDataRow IObjectQuerySerieBase.NextRow => NextRow;
        public ISingleDataRow<T> LastRow { get; set; }
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

        public string OriginalName => _originalName ?? Name;


        public QuerySerieBase(DateTime? startTime, DateTime? stopTime)
        {
            StartTime = startTime;
            StopTime = stopTime;
        }

        protected QuerySerieBase(IQuerySerieBase<T> serie)
        {
            StartTime = serie.StartTime;
            StopTime = serie.StopTime;
            Name = serie.Name;
            NextRow = serie.NextRow;
            PreviousRow = serie.PreviousRow;
        }
    }
}