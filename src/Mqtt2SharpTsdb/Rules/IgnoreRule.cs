using System.Runtime.Serialization;

namespace Mqtt2SharpTsdb.Rules
{
    [DataContract]
    public class IgnoreRule : Rule
    {
        public IgnoreRule(string topicName) : base(topicName)
        {
        }

        public IgnoreRule()
        {
        }
    }
}