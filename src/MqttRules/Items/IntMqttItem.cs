using System.Globalization;
using System.Linq;
using System.Text;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttRules.Items
{
    public class IntMqttItem : MqttItem<int?>
    {
        public IntMqttItem(string topic) : base(new[] { topic }, topic)
        {
        }

        public IntMqttItem(string subscribingTopic, string publishTopic) : base(new[] { subscribingTopic }, publishTopic)
        {
        }

        public IntMqttItem(string[] subscribingTopics, string publishTopic) : base(subscribingTopics, publishTopic)
        {
        }

        protected override int? ConvertValue(MqttMsgPublishEventArgs e)
        {
            if (e.Message.Any())
            {
                return int.Parse(Encoding.UTF8.GetString(e.Message));
            }
            return null;
        }

        protected override byte[] ConvertValueBack(int? itemValue)
        {
            return itemValue.HasValue ? Encoding.UTF8.GetBytes(itemValue.Value.ToString(CultureInfo.CurrentCulture)) : new byte[0];
        }


    }

    //public class IntMqttItem : MqttItem<int>
    //{
    //    public IntMqttItem(string[] subscribingTopics, string publishTopic) : base(subscribingTopics, publishTopic)
    //    {
    //    }

    //    protected override int ConvertValue(MqttMsgPublishEventArgs e)
    //    {
    //        return int.Parse(Encoding.UTF8.GetString(e.Message));
    //    }

    //    protected override byte[] ConvertValueBack(int itemValue)
    //    {
    //        return Encoding.UTF8.GetBytes(itemValue.ToString());
    //    }
    //}
}