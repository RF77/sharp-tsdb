using System.Runtime.Serialization;

namespace Mqtt2SharpTsdb.Rules
{
    [DataContract]
    public class Rule
    {
        [DataMember]
        public string TopicName { get; set; }

        [DataMember]
        public bool Handled { get; set; }

        public Rule(string topicName)
        {
            TopicName = topicName;
        }
    }
}
