using System;

namespace MqttRules.Attributes
{
    public class RuleSetAttribute : Attribute
    {
        public RuleSetAttribute()
        {
        }

        public RuleSetAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}