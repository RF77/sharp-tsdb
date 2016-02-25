using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SharpTsdbClient.Data;

namespace SharpTsdbClient
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Client
    {
        private readonly string _serverAddress;
        private readonly ushort _port;

        /// <summary>
        /// Create a client to access the Sharp TSDB Server
        /// </summary>
        /// <param name="serverAddress">address of Server, e.g. localhost</param>
        /// <param name="port">Port of server (defaults to 9003)</param>
        public Client(string serverAddress, ushort port = 9003)
        {
            _serverAddress = serverAddress;
            _port = port;
        }

        //public async Task WriteAsync(List<WritePoint> points, string name)
        //{
        //    var httpWebRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:9003/write?db=fux&meas={name}");
        //    httpWebRequest.ContentType = "application/json";
        //    httpWebRequest.Method = "POST";

        //    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //    {
        //        string json = new JavaScriptSerializer().Serialize(points);

        //        streamWriter.Write(json);
        //    }

        //    var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
        //    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //    {
        //        var result = await streamReader.ReadToEndAsync();
        //    }
        //}
    }
}
