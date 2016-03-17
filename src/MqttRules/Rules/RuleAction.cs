using System;
using System.Collections.Generic;

namespace MqttRules.Rules
{
    internal class RuleAction : IRuleLogic, IDisposable
    {
        protected readonly Rule ParentRule;
        public string RuleName { get; }

        public RuleAction(Rule parentRule, string ruleName)
        {
            ParentRule = parentRule;
            RuleName = ruleName;
        }

        protected List<Action> DisposingActions { get; } = new List<Action>();
        protected List<Func<TaskArguments, bool>> AdditionalFilters { get; } = new List<Func<TaskArguments, bool>>();

        public void Do(Action<TaskArguments> task)
        {
            ParentRule.Do(task);
        }

        public IRuleTrigger Or()
        {
            return ParentRule.Or();
        }

        public virtual void Dispose()
        {
            DisposingActions.ForEach(i => i.Invoke());
            DisposingActions.Clear();
        }
    }
}