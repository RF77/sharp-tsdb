using System.Runtime.Serialization;

namespace Mqtt2SharpTsdb.Rules
{
    [DataContract]
    public class RecordingRule : Rule
    {
        [DataMember]
        public string RecordingReason { get; set; }

        public RecordingRule(string topicName, string recordingReason) : base(topicName)
        {
            RecordingReason = recordingReason;
        }
    }
}