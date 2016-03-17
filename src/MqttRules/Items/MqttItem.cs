using System;
using System.Collections.Generic;
using System.Linq;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttRules.Items
{
    public abstract class MqttItem<TValue> : IMqttItem
    {
        public string PublishTopic { get; }
        public string[] SubscribingTopics { get; }
        public string Name { get; set; }
        public IList<string> Tags { get; set; } = new List<string>();
        public IList<IGroup> Groups { get; set; } = new List<IGroup>();
        private TValue _value;
        private TValue _setValue;
        private MqttClient _client;
        private byte _qosLevel;
        private bool _retain;

        public event Action<MqttItemChangedEventArgs<TValue>> ValueChanged; 

       

        protected MqttItem(string[] subscribingTopics, string publishTopic)
        {
            PublishTopic = publishTopic;
            SubscribingTopics = subscribingTopics;
        }

        public TValue Value
        {
            get { return _value; }
            set
            {
                if (!Equals(_value, value) || !Equals(_setValue, value))
                {
                    PublishValue(value, PublishTopic);
                }
            }
        }

        public void PublishValue(TValue value, string publishTopic)
        {
            SetValue = value;
            _client.Publish(publishTopic ?? PublishTopic ?? SubscribingTopics.First(), ConvertValueBack(value), _qosLevel, _retain);
        }

        public TValue SetValue
        {
            get { return _setValue; }
            set { _setValue = value; }
        }

        public void Connect(string brokerAddress, byte qosLevel = MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, bool retain = true)
        {
            _retain = retain;
            _client = new MqttClient(brokerAddress);

            // register to message received 
            _client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            string clientId = Guid.NewGuid().ToString();
            _client.Connect(clientId);
        
            // subscribe to the topic "/home/temperature" with QoS 2 
            _qosLevel = qosLevel;
            _client.Subscribe(SubscribingTopics, new[] { _qosLevel }); 
        }

        public void Disconnect()
        {
            try
            {
                _client.MqttMsgPublishReceived -= client_MqttMsgPublishReceived;
                _client.Unsubscribe(SubscribingTopics);
                _client.Disconnect();
            }
            finally
            {
                _client = null;
            }
        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var receivedValue = ConvertValue(e);
            if (!Equals(_value, receivedValue))
            {
                var oldValue = _value;
                _value = receivedValue;
                if (e.Retain == false)
                {
                    ValueChanged?.Invoke(new MqttItemChangedEventArgs<TValue>(this, oldValue, receivedValue)
                    {
                        MqttTopic = e.Topic
                    });
                }
            }
        }

        /// <summary>
        /// Convert from byte[] to target value
        /// </summary>
        /// <param name="e"></param>
        protected abstract TValue ConvertValue(MqttMsgPublishEventArgs e);

        /// <summary>
        /// Convert source value to byte[]
        /// </summary>
        /// <param name="itemValue"></param>
        protected abstract byte[] ConvertValueBack(TValue itemValue);

        object IMqttItem.Value
        {
            get { return Value; }
            set { Value = (TValue)value; }
        }
    }
}
