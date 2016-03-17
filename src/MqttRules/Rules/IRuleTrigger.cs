using System;
using System.Runtime.CompilerServices;
using MqttRules.Items;
using Quartz;

namespace MqttRules.Rules
{
    public interface IRuleTrigger
    {
        IRuleLogic StartUp();
        IRuleLogic CronTime(string cronTime, [CallerMemberName] string memberName = "");
        IRuleLogic Trigger(Func<TriggerBuilder, TriggerBuilder> trigger, [CallerMemberName] string memberName = "");
        IMqttRuleFilter<T> ItemChanged<T>(MqttItem<T> item, [CallerMemberName] string memberName = "");
        IStringMqttRuleFilter ItemChangedRegex(string itemAsRegex, [CallerMemberName] string memberName = "");
    }
}