using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Nancy.Json;
using SharpTsdbTypes.Communication;
using Timeenator.Interfaces;

namespace SharpTsdbClient
{
    public class MeasurementClient : ClientBase
    {
        public string MeasurementName { get; set; }

        public MeasurementClient(DbClient db, string measurementName)
        {
            Db = db;
            MeasurementName = measurementName;
        }

        /// <summary>
        /// Add points to a measurement
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="points">points to add</param>
        /// <param name="truncateDbToFirstElement">
        /// false: No check, if there are already points after the start of the new points
        /// true: The DB deletes all points after the timestamp of the first element of the data to add
        /// </param>
        /// <returns></returns>
        public async Task<string> AppendAsync<T>(IEnumerable<ISingleDataRow<T>> points, bool truncateDbToFirstElement)
        {
            //points = points.OrderBy(i => i.)
            return await PostRequestAsync<string>($"db/{Db.DbName}/{MeasurementName}/appendRows", new DataRows(points));
        }

        
    }
}