using System;

namespace MqttRules.Rules
{
    public interface IRuleTask
    {
        void Do(Action<TaskArguments> task);
    }
}