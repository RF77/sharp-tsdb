using System;

namespace MqttRules.Items
{
    public class MqttItemAttribute : Attribute
    {
        public string Topic { get; set; }

        public MqttItemAttribute()
        {
        }

        public MqttItemAttribute(string topic)
        {
            Topic = topic;
        }
    }
}