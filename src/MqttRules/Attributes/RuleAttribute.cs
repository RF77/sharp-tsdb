using System;

namespace MqttRules.Attributes
{
    public class RuleAttribute : Attribute
    {
        public RuleAttribute()
        {
        }

        public RuleAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}