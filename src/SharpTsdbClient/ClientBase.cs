using System.IO;
using System.Net;
using System.Threading.Tasks;
using Nancy.Json;
using Serialize.Linq.Serializers;

namespace SharpTsdbClient
{
    public class ClientBase
    {
        protected static readonly ExpressionSerializer LinqSerializer = new ExpressionSerializer(new BinarySerializer());
        private JavaScriptSerializer _scriptSerializer = new JavaScriptSerializer();

        public DbClient Db { get; set; }

        protected async Task<T> PostRequestAsync<T>(string url, object objectToserialize, bool asJson = true)
        {
            var httpWebRequest =
                (HttpWebRequest)
                    WebRequest.Create($"http://{Db.Client.ServerAddress}:{Db.Client.Port}/{url}");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                if (asJson)
                {
                    string json = _scriptSerializer.Serialize(objectToserialize);
                    streamWriter.Write(json);
                }
                else
                {
                    streamWriter.Write(objectToserialize);
                }
            }

            var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = await streamReader.ReadToEndAsync();
                _scriptSerializer.Deserialize<T>(result);
            }
            return default(T);
        }

        protected async Task<string> GetRequestAsync(string url)
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