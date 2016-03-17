using System;
using System.Linq;
using System.Text;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttRules.Items
{
    public class DateTimeMqttItem : MqttItem<DateTime?>
    {
        public DateTimeMqttItem(string topic) : base(new[] { topic }, topic)
        {
        }

        public DateTimeMqttItem(string subscribingTopic, string publishTopic) : base(new[] { subscribingTopic }, publishTopic)
        {
        }

        public DateTimeMqttItem(string[] subscribingTopics, string publishTopic) : base(subscribingTopics, publishTopic)
        {
        }

        protected override DateTime? ConvertValue(MqttMsgPublishEventArgs e)
        {
            if (e.Message.Any())
            {
                return DateTime.Parse(Encoding.UTF8.GetString(e.Message));
            }
            return null;
        }

        protected override byte[] ConvertValueBack(DateTime? itemValue)
        {
            return itemValue.HasValue ? Encoding.UTF8.GetBytes(itemValue.Value.ToString()) : new byte[0];
        }


    }
}