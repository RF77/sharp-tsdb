using System;

namespace MqttRules.Rules
{
    public interface IMqttRuleFilter<T> : IRuleLogic
    {
        IMqttRuleFilter<T> From(T fromValue);
        IMqttRuleFilter<T> To(T toValue);
        IMqttRuleFilter<T> If(Func<T, T, bool> ifPredicate);
        IMqttRuleFilter<T> If(Func<bool> ifPredicate);
    }
}