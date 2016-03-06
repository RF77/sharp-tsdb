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
using System.Collections.Generic;
using System.Diagnostics;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    [DebuggerDisplay("{FullName}")]
    public abstract class QuerySerieBase<T> : IQuerySerieBase<T> where T : struct
    {
        private string _name;
        private string _originalName;

        public QuerySerieBase(DateTime? startTime, DateTime? endTime)
        {
            StartTime = startTime?.ToUniversalTime();
            EndTime = endTime?.ToUniversalTime();
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

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public string FullName
        {
            get
            {
                if (GroupName != null)
                {
                    if (Key != null)
                    {
                        return $"{GroupName}.{Key}";
                    }
                    else
                    {
                        return GroupName;
                    }
                }
                else if (Key != null)
                {
                    return Key;
                }
                return $"{_name}";
            }
        }

        /// <summary>
        ///     Last value before the start time or null
        /// </summary>
        public ISingleDataRow<T> PreviousRow { get; set; }

        IObjectSingleDataRow IObjectQuerySerieBase.NextRow => NextRow;
        public ISingleDataRow<T> LastRow { get; set; }
        public abstract object this[int index] { get; set; }
        IObjectSingleDataRow IObjectQuerySerieBase.LastRow => LastRow;
        IObjectSingleDataRow IObjectQuerySerieBase.PreviousRow => PreviousRow;

        /// <summary>
        ///     first value after end time or null
        /// </summary>
        public ISingleDataRow<T> NextRow { get; set; }

        /// <summary>
        ///     Name of measurement
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

        protected void SetAlias(string name)
        {
            if (GroupName != null)
            {
                Key = name;
            }
            else
            {
                Name = name;
            }
        }

        public abstract IEnumerable<IObjectQuerySerie> Series { get; }
    }
}