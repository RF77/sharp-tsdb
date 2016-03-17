using System.Linq;
using System.Text;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttRules.Items
{
    public class BoolMqttItem : MqttItem<bool?>
    {
        public BoolMqttItem(string topic) : base(new[] { topic }, topic)
        {
        }

        public BoolMqttItem(string subscribingTopic, string publishTopic) : base(new[] { subscribingTopic }, publishTopic)
        {
        }

        public BoolMqttItem(string[] subscribingTopics, string publishTopic) : base(subscribingTopics, publishTopic)
        {
        }

        protected override bool? ConvertValue(MqttMsgPublishEventArgs e)
        {
            if (e.Message.Any())
            {
                var val = Encoding.UTF8.GetString(e.Message);
                return val == "ON";
            }
            return null;
        }

        protected override byte[] ConvertValueBack(bool? itemValue)
        {
            return itemValue.HasValue ? Encoding.UTF8.GetBytes(itemValue.Value ? "ON" : "OFF") : new byte[0];
        }


    }
}