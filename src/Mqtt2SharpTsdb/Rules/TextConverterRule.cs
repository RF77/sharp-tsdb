using System.Runtime.Serialization;

namespace Mqtt2SharpTsdb.Rules
{
    [DataContract]
    public class TextConverterRule : Rule
    {
        [DataMember]
        public object SourceValue { get; set; }

        [DataMember]
        public object ConvertedValue { get; set; }

        public TextConverterRule(string sourceValue, string convertedValue) : base(null)
        {
            SourceValue = sourceValue;
            ConvertedValue = convertedValue;
        }

        public TextConverterRule()
        {
        }

        public object Replace(object val)
        {
            if (val == SourceValue)
            {
                return ConvertedValue;
            }
            return val;
        }
    }
}