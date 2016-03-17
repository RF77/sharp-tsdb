using System.Runtime.Serialization;

namespace Mqtt2SharpTsdb.Rules
{

    [DataContract]
    public class NamingRule : Rule
    {
        [DataMember]
        public string MeasurementName { get; set; }

        public NamingRule(string topicName, string measurementName) : base(topicName)
        {
            MeasurementName = measurementName;
        }
    }
}
