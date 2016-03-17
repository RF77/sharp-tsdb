using System;

namespace MqttRules.Rules
{
    internal class DisposingAction
    {
        internal Action Action { get; set; }
        internal string Name { get; set; }

        public DisposingAction(Action action, string name)
        {
            Action = action;
            Name = name;
        }
    }
}