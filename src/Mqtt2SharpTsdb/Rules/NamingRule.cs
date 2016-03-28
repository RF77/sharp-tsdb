using System.Runtime.Serialization;
using System.Text.RegularExpressions;

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

        public NamingRule()
        {
        }

        public bool Replace(ref string topic)
        {
            if (Regex.IsMatch(topic, TopicName))
            {
                topic = Regex.Replace(topic, TopicName, MeasurementName);
                return Handled;
            }
            return false;
        }
    }
}
