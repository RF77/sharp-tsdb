using System.Runtime.Serialization;

namespace Mqtt2SharpTsdb.Rules
{
    [DataContract]
    public class TextConverterRule : Rule
    {

        [DataMember]
        public long ConvertedValue { get; set; }

        public TextConverterRule(string topicName, long convertedValue) : base(topicName)
        {
            ConvertedValue = convertedValue;
        }
    }
}