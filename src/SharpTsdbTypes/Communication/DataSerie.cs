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

using System.Linq;
using System.Runtime.Serialization;
using Timeenator.Extensions;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace SharpTsdbTypes.Communication
{
    [DataContract]
    public class DataSerie
    {
        public DataSerie()
        {
        }

        public DataSerie(IObjectQuerySerie querySerie)
        {
            Rows = new DataRows(querySerie.Rows);
            Name = querySerie.FullName;
            StartTime = querySerie.StartTime.ToFileTimeUtc();
            EndTime = querySerie.EndTime.ToFileTimeUtc();
            PreviousRow = querySerie.PreviousRow?.ToArray();
            NextRow = querySerie.NextRow?.ToArray();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public long? StartTime { get; set; }

        [DataMember]
        public long? EndTime { get; set; }

        /// <summary>
        ///     Last value before the start time or null
        /// </summary>
        [DataMember]
        public object[] PreviousRow { get; set; }

        [DataMember]
        public object[] NextRow { get; set; }

        [DataMember]
        public DataRows Rows { get; set; }

        public IQuerySerie<T> ToQuerySerie<T>() where T : struct
        {
            return new QuerySerie<T>(Rows.AsTyped<T>().ToList(), StartTime.FromFileTimeUtcToDateTimeUtc(),
                EndTime.FromFileTimeUtcToDateTimeUtc())
            {
                PreviousRow = PreviousRow.ToSingleDataRow<T>(),
                NextRow = NextRow.ToSingleDataRow<T>()
            };
        }

        public INullableQuerySerie<T> ToNullableQuerySerie<T>() where T : struct
        {
            return new NullableQuerySerie<T>(Rows.AsNullableTyped<T>().ToList(),
                StartTime.FromFileTimeUtcToDateTimeUtc(), EndTime.FromFileTimeUtcToDateTimeUtc())
            {
                PreviousRow = PreviousRow?.ToSingleDataRow<T>(),
                NextRow = NextRow?.ToSingleDataRow<T>(),
                Name = Name
            };
        }
    }
}