using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MqttRules.Rules
{
    public class RulesBase : IDisposable
    {
        private List<Rule> _rules = new List<Rule>();

        protected IRuleTrigger On
        {
            get
            {
                StackTrace stackTrace = new StackTrace();           // get call stack
                StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)
                var methodBase = stackFrames[1].GetMethod();
                var ruleName = methodBase.Name; //TODO Maybe class name and method name?

                var rule = new Rule(ruleName);

                _rules.Add(rule);
                return rule;
            }
        }

        public void Dispose()
        {
            _rules.ForEach(i => i.Dispose());
        }
        public void Dispose(string name)
        {
            foreach (var source in _rules.Where(i => i.RuleName == name))
            {
                source.Dispose();
            }
        }
    }
}
