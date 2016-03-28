using Mqtt2SharpTsdb.Config;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Mqtt2SharpTsdb.Items
{
    public interface IMeasurementItem
    {
        RuleConfiguration RuleConfiguration { get; set; }
        void ReceivedValue(object val, MqttMsgPublishEventArgs mqttMsgPublishEventArgs);
        void Flush();
    }
}