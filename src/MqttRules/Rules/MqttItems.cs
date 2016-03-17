using MqttRules.Items;

namespace MqttRules.Rules
{
    public static class MqttItems
    {
        private static StringMqttItem _listenToEverythingItemInstance;
        public static StringMqttItem ListenToEverythingItem => _listenToEverythingItemInstance ??
                                                               (_listenToEverythingItemInstance = new StringMqttItem(new[] {"#"}, null));
    }
}