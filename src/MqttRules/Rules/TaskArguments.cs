using MqttRules.Items;
using Quartz;

namespace MqttRules.Rules
{
    public class TaskArguments
    {
        public MqttItemChangedEventArgs ItemChangedArgs { get; set; }

        public bool IsTimeTrigger => JobExecutionContext != null;

        public IJobExecutionContext JobExecutionContext { get; set; }
    }
}