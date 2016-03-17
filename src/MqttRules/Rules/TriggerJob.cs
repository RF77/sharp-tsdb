using System;
using Quartz;

namespace MqttRules.Rules
{
    public class TriggerJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ((TriggerAction)context.JobDetail.JobDataMap["instance"]).Execute(context);
        }
    }
}