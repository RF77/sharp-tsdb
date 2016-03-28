using System.Collections.Generic;
using System.Runtime.Serialization;
using Mqtt2SharpTsdb.Rules;

namespace Mqtt2SharpTsdb.Config
{
    [DataContract]
    public class RuleConfiguration
    {
        [DataMember]
        public List<NamingRule> NamingRules { get; set; } = new List<NamingRule>();

        [DataMember]
        public List<IgnoreRule> IgnoringRules { get; set; } = new List<IgnoreRule>();

        [DataMember]
        public List<TypeRule> TypeRules { get; set; } = new List<TypeRule>();

        [DataMember]
        public List<TextConverterRule> TextConverterRules { get; set; } = new List<TextConverterRule>();

        [DataMember]
        public List<JsonConverterRule> JsonConverterRules { get; set; } = new List<JsonConverterRule>();

        [DataMember]
        public List<RecordingRule> RecordingRules { get; set; } = new List<RecordingRule>();


    }
}
