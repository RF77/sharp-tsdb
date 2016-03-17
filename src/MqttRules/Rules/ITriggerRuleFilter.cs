using System;

namespace MqttRules.Rules
{
    public interface ITriggerRuleFilter : IRuleLogic
    {
        ITriggerRuleFilter If(Func<bool> ifPredicate);
    }
}