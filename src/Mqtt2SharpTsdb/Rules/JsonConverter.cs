using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mqtt2SharpTsdb.Rules
{
    public class JsonConverter
    {
        public string JsonProperty { get; }

        public JsonConverter(string jsonProperty)
        {
            JsonProperty = jsonProperty;
        }

        public object Convert(string message)
        {
            var data = JsonConvert.DeserializeObject(message) as JObject;
            JToken test = data?[JsonProperty];
            var val = test?.ToObject<object>();
            return val;
        }
    }
}