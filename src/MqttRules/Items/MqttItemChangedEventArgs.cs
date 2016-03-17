using System;

namespace MqttRules.Items
{
    public class MqttItemChangedEventArgs<T> : EventArgs
    {
        public MqttItem<T> Item { get; set; }
        public T OldValue { get; set; }
        public T NewValue { get; set; }
        public string MqttTopic { get; set; }

        public MqttItemChangedEventArgs(MqttItem<T> item, T oldValue, T newValue)
        {
            Item = item;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class MqttItemChangedEventArgs : EventArgs
    {
        public IMqttItem Item { get; set; }
        public string MqttTopic { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }

        public MqttItemChangedEventArgs(IMqttItem item, object oldValue, object newValue)
        {
            Item = item;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}