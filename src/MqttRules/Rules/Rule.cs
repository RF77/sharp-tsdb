using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MqttRules.Items;
using Quartz;
using Quartz.Impl;

namespace MqttRules.Rules
{
    public class Rule:IRuleTrigger, IRuleLogic, IDisposable
    {
        public string RuleName { get; }
        private readonly List<RuleAction> _ruleActions = new List<RuleAction>();
        private bool _disposed;
        private Action<TaskArguments> _task;

        private IScheduler _scheduler;
        private ISchedulerFactory _schedulerFactory;

        private IScheduler Scheduler
        {
            get
            {
                if (_scheduler == null)
                {
                    _schedulerFactory = new StdSchedulerFactory();
                    _scheduler = _schedulerFactory.GetScheduler();
                    _scheduler.Start();
                }
                return _scheduler;
            }
        }

        public Rule(string ruleName)
        {
            RuleName = ruleName;
        }

        public IRuleLogic StartUp()
        {
            Task.Run(() => RunTask(new TaskArguments()));
            return this;
        }

        public IRuleLogic CronTime(string cronTime, [CallerMemberName] string memberName = "")
        {
            return Trigger(t => t.WithCronSchedule(cronTime));
        }

        public IRuleLogic Trigger(Func<TriggerBuilder, TriggerBuilder> trigger, [CallerMemberName] string memberName = "")
        {
            var itemAction = new TriggerAction(this, memberName, Scheduler, trigger);
            _ruleActions.Add(itemAction);
            return itemAction;
        }

        public IMqttRuleFilter<T> ItemChanged<T>(MqttItem<T> item, [CallerMemberName] string memberName = "")
        {
            var itemAction = new MqttItemRuleAction<T>(this, memberName, item);
            _ruleActions.Add(itemAction);
            return itemAction;
        }

        public IStringMqttRuleFilter ItemChangedRegex(string itemAsRegex, string memberName = "")
        {
            var itemAction = new StringMqttRuleAction(this, memberName, MqttItems.ListenToEverythingItem, itemAsRegex);
            _ruleActions.Add(itemAction);
            return itemAction;
        }

        public void Do(Action<TaskArguments> task)
        {
            _task = task;
        }

        public IRuleTrigger Or()
        {
            return this;
        }

        internal void RunTask(TaskArguments taskArguments)
        {
            if (_task == null)
            {
                //Maybe the task is not yet configured
                Thread.Sleep(100);
            }
            _task?.Invoke(taskArguments);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (var ruleAction in _ruleActions)
                {
                    ruleAction.Dispose();
                }
                _ruleActions.Clear();
                _disposed = true;
            }
        }

        public void Dispose(string name)
        {
            foreach (var disposingAction in _ruleActions.Where(i => i.RuleName == name))
            {
                disposingAction.Dispose();
            }
            _ruleActions.RemoveAll(i => i.RuleName == name);
        }
    }
}