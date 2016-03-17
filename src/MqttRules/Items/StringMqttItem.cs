using System.Text;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttRules.Items
{
    public class StringMqttItem : MqttItem<string>
    {
        public StringMqttItem(string topic) : base(new[] { topic }, topic)
        {
        }

        public StringMqttItem(string subscribingTopic, string publishTopic) : base(new[] { subscribingTopic }, publishTopic)
        {
        }

        public StringMqttItem(string[] subscribingTopics, string publishTopic) : base(subscribingTopics, publishTopic)
        {
        }

        protected override string ConvertValue(MqttMsgPublishEventArgs e)
        {
            return Encoding.UTF8.GetString(e.Message);
        }

        protected override byte[] ConvertValueBack(string itemValue)
        {
            return Encoding.UTF8.GetBytes(itemValue);
        }
    }
}