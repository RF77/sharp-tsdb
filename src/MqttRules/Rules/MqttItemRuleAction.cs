using System;
using MqttRules.Items;

namespace MqttRules.Rules
{
    internal class MqttItemRuleAction<T> : RuleAction, IMqttRuleFilter<T>
    {
        private MqttItem<T> _item;

        internal virtual void OnItemChanged(MqttItemChangedEventArgs<T> args)
        {
            var taskArguments = new TaskArguments { ItemChangedArgs = new MqttItemChangedEventArgs(args.Item, args.OldValue, args.NewValue) };
            CheckForAdditionalFilterAndRunTask(taskArguments);
        }

        protected void CheckForAdditionalFilterAndRunTask(TaskArguments taskArguments)
        {
            if (AdditionalFilters.TrueForAll(func => func(taskArguments)))
            {
                ParentRule.RunTask(taskArguments);
            }
        }

        public MqttItemRuleAction(Rule parentRule, string ruleName, MqttItem<T> item) : base(parentRule, ruleName)
        {
            _item = item;
            item.ValueChanged += OnItemChanged;
            DisposingActions.Add(() => item.ValueChanged -= OnItemChanged);
        }


        public override void Dispose()
        {
            base.Dispose();
            _item = null;
        }

        public IMqttRuleFilter<T> From(T fromValue)
        {
            AdditionalFilters.Add(args => Equals((T)args.ItemChangedArgs.OldValue, fromValue));
            return this;
        }

        public IMqttRuleFilter<T> To(T toValue)
        {
            AdditionalFilters.Add(args => Equals((T)args.ItemChangedArgs.NewValue, toValue));
            return this;
        }

        public IMqttRuleFilter<T> If(Func<T, T, bool> ifPredicate)
        {
            AdditionalFilters.Add(args => ifPredicate((T)args.ItemChangedArgs.OldValue, (T)args.ItemChangedArgs.NewValue));
            return this;
        }

        public IMqttRuleFilter<T> If(Func<bool> ifPredicate)
        {
            AdditionalFilters.Add(args => ifPredicate());
            return this;
        }
    }
}