using System.Text.RegularExpressions;
using MqttRules.Items;

namespace MqttRules.Rules
{
    internal class StringMqttRuleAction : MqttItemRuleAction<string>, IStringMqttRuleFilter
    {
        public string ItemAsRegex { get; set; }

        public StringMqttRuleAction(Rule parentRule, string ruleName, MqttItem<string> item, string itemAsRegex) : base(parentRule, ruleName, item)
        {
            ItemAsRegex = itemAsRegex;
        }

        internal override void OnItemChanged(MqttItemChangedEventArgs<string> args)
        {
            if (Regex.Match(args.MqttTopic, ItemAsRegex).Success)
            {
                var taskArguments = new TaskArguments { ItemChangedArgs = new MqttItemChangedEventArgs(args.Item, args.OldValue, args.NewValue)
                    {
                        MqttTopic = args.MqttTopic
                    }
                };
                CheckForAdditionalFilterAndRunTask(taskArguments);
            }
        }
    }
}