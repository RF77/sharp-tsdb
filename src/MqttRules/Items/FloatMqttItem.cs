using System.Globalization;
using System.Linq;
using System.Text;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttRules.Items
{
    public class FloatMqttItem : MqttItem<float?>
    {
        public FloatMqttItem(string topic) : base(new[] { topic }, topic)
        {
        }

        public FloatMqttItem(string subscribingTopic, string publishTopic) : base(new[] { subscribingTopic }, publishTopic)
        {
        }

        public FloatMqttItem(string[] subscribingTopics, string publishTopic) : base(subscribingTopics, publishTopic)
        {
        }

        protected override float? ConvertValue(MqttMsgPublishEventArgs e)
        {
            if (e.Message.Any())
            {
                return float.Parse(Encoding.UTF8.GetString(e.Message));
            }
            return null;
        }

        protected override byte[] ConvertValueBack(float? itemValue)
        {
            return itemValue.HasValue ? Encoding.UTF8.GetBytes(itemValue.Value.ToString(CultureInfo.CurrentCulture)) : new byte[0];
        }


    }
}