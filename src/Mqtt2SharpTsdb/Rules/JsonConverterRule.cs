using System.Runtime.Serialization;

namespace Mqtt2SharpTsdb.Rules
{
    [DataContract]
    public class JsonConverterRule : Rule
    {

        [DataMember]
        public string JsonPropertyName { get; set; }

        public JsonConverterRule(string topicName, string jsonPropertyName) : base(topicName)
        {
            JsonPropertyName = jsonPropertyName;
        }

        public JsonConverterRule()
        {
        }
    }
}