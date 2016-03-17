using System;
using Quartz;

namespace MqttRules.Rules
{
    internal class TriggerAction : RuleAction, IJob, ITriggerRuleFilter
    {
        private readonly IScheduler _scheduler;
        private IJobDetail _jobDetail;
        private readonly ITrigger _trigger;

        public TriggerAction(Rule parentRule, string ruleName, IScheduler scheduler, Func<TriggerBuilder, TriggerBuilder> triggerAction) : base(parentRule, ruleName)
        {
            _scheduler = scheduler;
            _jobDetail = JobBuilder.Create<TriggerJob>()
                .WithIdentity(GetHashCode().ToString(), ruleName)
                .Build();
            _jobDetail.JobDataMap["instance"] = this;

            var trigBuilder = TriggerBuilder.Create()
                .WithIdentity(GetHashCode().ToString(), ruleName);
            _trigger = triggerAction(trigBuilder).Build();

            _scheduler.ScheduleJob(_jobDetail, _trigger);
        }


        public override void Dispose()
        {
            base.Dispose();
            _scheduler?.UnscheduleJob(_trigger.Key);
        }

        public ITriggerRuleFilter If(Func<bool> ifPredicate)
        {
            AdditionalFilters.Add(args => ifPredicate());
            return this;
        }

        public void Execute(IJobExecutionContext context)
        {
            var taskArguments = new TaskArguments { JobExecutionContext = context};
            if (AdditionalFilters.TrueForAll(func => func(taskArguments)))
            {
                ParentRule.RunTask(taskArguments);
            }
        }
    }
}