using System.Collections.Generic;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttRules.Items
{
    public interface IMqttItem
    {
        object Value { get; set; }
        string PublishTopic { get; }
        string[] SubscribingTopics { get; }
        string Name { get; set; }
        IList<string> Tags { get; }
        IList<IGroup> Groups { get; } 
        void Connect(string brokerAddress, byte qosLevel = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, bool retain = true);
        void Disconnect();
    }
}