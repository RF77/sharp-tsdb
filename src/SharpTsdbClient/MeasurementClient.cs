using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Nancy.Json;
using Timeenator.Interfaces;

namespace SharpTsdbClient
{
    public class MeasurementClient
    {
        public DbClient Db { get; set; }
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
            return await PostRequestAsync($"db /{Db.DbName}/{MeasurementName}/appendRows", points);
        }

        private async Task<string> PostRequestAsync(string url, object objectToserialize)
        {
            var httpWebRequest =
                (HttpWebRequest)
                    WebRequest.Create($"http://{Db.Client.ServerAddress}:{Db.Client.Port}/{url}");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(objectToserialize);

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse) await httpWebRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return await streamReader.ReadToEndAsync();
            }
        }

        private async Task<string> GetRequestAsync(string url)
        {
            var httpWebRequest =
                (HttpWebRequest)
                    WebRequest.Create($"http://{Db.Client.ServerAddress}:{Db.Client.Port}/{url}");
            //httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    string json = new JavaScriptSerializer().Serialize(objectToserialize);

            //    streamWriter.Write(json);
            //}

            var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return await streamReader.ReadToEndAsync();
            }
        }
    }
}