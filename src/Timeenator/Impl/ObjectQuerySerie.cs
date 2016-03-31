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
    public class ObjectQuerySerie : IObjectQuerySerie, ITimeWriteableQuerySerie
    {
        private string _name;
        private string _originalName;

        public ObjectQuerySerie(DateTime? startTime, DateTime? endTime)
        {
            StartTime = startTime?.ToUniversalTime();
            EndTime = endTime?.ToUniversalTime();
        }
        public ObjectQuerySerie(IReadOnlyList<IObjectSingleDataRow> rows, DateTime? startTime, DateTime? endTime)
        {
            StartTime = startTime?.ToUniversalTime();
            EndTime = endTime?.ToUniversalTime();
            Rows = rows;
        }

        public ObjectQuerySerie(IObjectQuerySerie serie)
        {
            StartTime = serie.StartTime;
            EndTime = serie.EndTime;
            Name = serie.Name;
            NextRow = serie.NextRow;
            PreviousRow = serie.PreviousRow;
            LastRow = serie.LastRow;
            GroupName = serie.GroupName;
            Key = serie.Key;
            IsDbSerie = serie.IsDbSerie;
        }

        public bool IsDbSerie { get; set; }

        /// <summary>
        ///     Last value before the start time or null
        /// </summary>
        public IObjectSingleDataRow PreviousRow { get; set; }

        public IObjectSingleDataRow LastRow { get; set; }

        /// <summary>
        ///     first value after end time or null
        /// </summary>
        public IObjectSingleDataRow NextRow { get; set; }

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
                    return GroupName;
                }
                if (Key != null)
                {
                    return Key;
                }
                return $"{_name}";
            }
        }

        public IReadOnlyList<IObjectSingleDataRow> Rows { get; set; }
        IObjectSingleDataRow IObjectQuerySerieBase.NextRow => NextRow;
        public virtual object this[int index]
        {
            get { return Rows[index].Value; }
            set
            {
                if (value == null)
                {
                    Rows[index].Value = null;
                }
                else
                {
                    Rows[index].Value = value;
                }
            }
        }
        IObjectSingleDataRow IObjectQuerySerieBase.LastRow => LastRow;
        IObjectSingleDataRow IObjectQuerySerieBase.PreviousRow => PreviousRow;

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
        public virtual IEnumerable<IObjectQuerySerie> Series => new[] { this };

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
    }
}