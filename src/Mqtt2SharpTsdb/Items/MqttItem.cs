using System;
using System.Linq;
using System.Reflection;
using log4net;
using Mqtt2SharpTsdb.Config;
using Mqtt2SharpTsdb.Rules;
using Timeenator.Impl;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Mqtt2SharpTsdb.Items
{
    public class MqttItem
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly RuleConfiguration _ruleConfiguration;
        public string Topic { get; }

        private readonly IMeasurementItem _measurement;

        public MqttItem(string topic, RuleConfiguration ruleConfiguration)
        {
            _ruleConfiguration = ruleConfiguration;
            Topic = topic;
            if (!HaveToIgnore())
            {
                CreateJsonValueConverter(Topic);
                var measurementName = GetMeasurementName();
                var type = GetMeasurementType(measurementName);
                _measurement = CreateMeasurement(measurementName, type);
                _measurement.RuleConfiguration = ruleConfiguration;
            }
        }

        private IMeasurementItem CreateMeasurement(string measurementName, string type)
        {
            switch (type)
            {
                case "float":
                    return new MeasurementItem<float>(measurementName);
                case "byte":
                    return new MeasurementItem<byte>(measurementName);
                case "long":
                    return new MeasurementItem<long>(measurementName);
                case "int":
                    return new MeasurementItem<int>(measurementName);
            }
            throw new NotImplementedException();
        }

        private string GetMeasurementType(string measurementName)
        {
            foreach (TypeRule rule in _ruleConfiguration.TypeRules)
            {
                if (rule.MatchTopic(measurementName))
                {
                    return rule.ValueType;
                }
            }
            return "float";
        }

        private void CreateJsonValueConverter(string topic)
        {
            foreach (var rule in _ruleConfiguration.JsonConverterRules)
            {
                if (rule.MatchTopic(topic))
                {
                    JsonConverter = new JsonConverter(rule.JsonPropertyName);
                }
            }
        }

        public JsonConverter JsonConverter { get; set; }


        private bool HaveToIgnore()
        {
            return _ruleConfiguration.IgnoringRules.Any(rule => rule.MatchTopic(Topic));
        }

        private string GetMeasurementName()
        {
            var measName = Topic;
            foreach (NamingRule rule in _ruleConfiguration.NamingRules)
            {
                if (rule.Replace(ref measName))
                {
                    break;
                }
            }
            return measName;
        }

        public void ReceivedMessage(string message, MqttMsgPublishEventArgs mqttMsgPublishEventArgs)
        {
            object val = message;
            try
            {
                if (_measurement != null)
                {
                    if (JsonConverter != null)
                    {
                        val = JsonConverter.Convert(message);
                    }

                    foreach (var textConverterRule in _ruleConfiguration.TextConverterRules)
                    {
                        if (Equals(textConverterRule.SourceValue, val))
                        {
                            val = textConverterRule.ConvertedValue;
                            break;
                        }
                    }

                    _measurement.ReceivedValue(val, mqttMsgPublishEventArgs);

                }


            }
            catch (Exception ex)
            {
                Logger.Warn(
                    $"Excpetion in topic {Topic}, content: {message}, reason: {ex.Message}");
            }
        }
    }
}