using System;
using System.Runtime.Serialization;

namespace Mqtt2SharpTsdb.Rules
{
    [DataContract]
    public class TypeRule : Rule
    {

        [DataMember]
        public string ValueType { get; set; }

        public TypeRule(string topicName, string valueType) : base(topicName)
        {
            ValueType = valueType;
        }

        public TypeRule()
        {
        }
    }
}