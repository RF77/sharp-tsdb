using System.Runtime.Serialization;

namespace Mqtt2SharpTsdb.Rules
{
    [DataContract]
    public class TextConverterRule : Rule
    {
        [DataMember]
        public string SourceValue { get; set; }

        [DataMember]
        public string ConvertedValue { get; set; }

        public TextConverterRule(string sourceValue, string convertedValue) : base(null)
        {
            SourceValue = sourceValue;
            ConvertedValue = convertedValue;
        }

        public TextConverterRule()
        {
        }
    }
}