// /*******************************************************************************
//  * Copyright (c) 2016 by RF77 (https://github.com/RF77)
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the Eclipse Public License v1.0
//  * which accompanies this distribution, and is available at
//  * http://www.eclipse.org/legal/epl-v10.html
//  *
//  * Contributors:
//  *    RF77 - initial API and implementation and/or initial documentation
//  *******************************************************************************/ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using Mqtt2SharpTsdb.Config;
using SharpTsdbClient;
using Timeenator.Extensions;
using Timeenator.Impl;
using Timeenator.Interfaces;
using uPLibrary.Networking.M2Mqtt.Messages;

// ReSharper disable StaticMemberInGenericType

namespace Mqtt2SharpTsdb.Items
{
    public class MeasurementItem<T> : IMeasurementItem where T : struct
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly DbClient DbClient = new DbClient(new Client("10.10.1.77"), "raspberry");
        private bool _isFlushing;
        private TimeSpan? _minInterval;
        private bool _onlyChanges;
        private RuleConfiguration _ruleConfiguration;

        public MeasurementItem(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public ISingleDataRow<T> CurrentItem { get; private set; }
        public T? CurrentValue => CurrentItem?.Value;
        public DateTime? LastChange => CurrentItem?.TimeUtc;

        /// <summary>
        ///     Theses items couldn't be written successfully to DB so far
        /// </summary>
        public IList<ISingleDataRow<T>> QueuedItems { get; } = new List<ISingleDataRow<T>>();

        public RuleConfiguration RuleConfiguration
        {
            get { return _ruleConfiguration; }
            set
            {
                _ruleConfiguration = value;
                if (value != null)
                {
                    foreach (var rule in value.RecordingRules)
                    {
                        if (rule.MatchTopic(Name))
                        {
                            if (rule.RecordingReason.ToLower().StartsWith("change"))
                            {
                                _onlyChanges = true;
                            }
                            else if (!string.IsNullOrWhiteSpace(rule.RecordingReason))
                            {
                                _minInterval = rule.RecordingReason.ToTimeSpan();
                            }
                            break;
                        }
                    }
                }
            }
        }

        public void ReceivedValue(object val, MqttMsgPublishEventArgs mqttMsgPublishEventArgs)
        {
            val = RuleConfiguration.TextConverterRules.Aggregate(val, (current, rule) => rule.Replace(current));

            AddValue((T) Convert.ChangeType(val, typeof(T)), mqttMsgPublishEventArgs);
        }

        public void AddValue(T val, MqttMsgPublishEventArgs mqttMsgPublishEventArgs)
        {
            lock (this)
            {
                if (_onlyChanges && Equals(CurrentItem?.Value, val))
                {
                    return;
                }
                var now = DateTime.UtcNow;
                if (_minInterval != null)
                {
                    var diff = now - (CurrentItem?.TimeUtc ?? DateTime.MinValue);
                    if (diff < _minInterval)
                    {
                        return;
                    }
                }
                CurrentItem = new SingleDataRow<T>(now, val);
                if (!mqttMsgPublishEventArgs.Retain)
                {
                    QueuedItems.Add(CurrentItem);
                    Flush();
                }
            }
        }

        public async void Flush()
        {
            if (!_isFlushing && QueuedItems.Any())
            {
                try
                {
                    _isFlushing = true;
                    await DbClient.Measurement(Name).AppendAsync(QueuedItems, false);
                    Logger.Debug($"{Name}: Wrote {QueuedItems.Count} items: {string.Join(", ", QueuedItems.Select(i => $"{i.TimeUtc}: {i.Value}" ))}");
                    QueuedItems.Clear();
                }
                catch (Exception ex)
                {
                    Logger.Warn($"Couldn't write data for item {Name}, now queued {QueuedItems.Count} items, Reason: {ex.Message}");
                    ItemsToFlush.AddItem(this);
                }
                finally
                {
                    _isFlushing = false;
                }
            }
        }
    }
}