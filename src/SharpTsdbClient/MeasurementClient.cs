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
using System.Threading.Tasks;
using SharpTsdbTypes.Communication;
using Timeenator.Extensions;
using Timeenator.Interfaces;

namespace SharpTsdbClient
{
    public class MeasurementClient : ClientBase
    {
        public MeasurementClient(DbClient db, string measurementName)
        {
            Db = db;
            MeasurementName = measurementName;
        }

        public string MeasurementName { get; set; }

        /// <summary>
        ///     Add points to a measurement
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="points">points to add</param>
        /// <param name="truncateDbToFirstElement">
        ///     false: No check, if there are already points after the start of the new points
        ///     true: The DB deletes all points after the timestamp of the first element of the data to add
        /// </param>
        /// <returns></returns>
        public async Task<string> AppendAsync<T>(IEnumerable<ISingleDataRow<T>> points, bool truncateDbToFirstElement)
        {
            //points = points.OrderBy(i => i.)
            string url = $"db/{Db.DbName}/{MeasurementName}/appendRows?type={typeof (T).ToShortCode()}";
            if (truncateDbToFirstElement)
            {
                url += "&truncateDbToFirstElement=true";
            }
            return await PostRequestAsync<string>(url, new DataRows(points));
        }

        public async Task<string> ClearMeasurementAsync(DateTime? after = null)
        {
            return
                await
                    GetRequestAsync($"db/{Db.DbName}/clearMeasurment/{MeasurementName}?after={after?.ToFileTimeUtc()}");
        }
    }
}