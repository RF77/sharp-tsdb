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
using Timeenator.Impl;
using Timeenator.Interfaces;
// ReSharper disable StaticMemberInGenericType

namespace Mqtt2SharpTsdb.Items
{
    public class MeasurementItem<T> : IMeasurementItem where T : struct
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly DbClient DbClient = new DbClient(new Client("10.10.1.9"), "Haus");
        private bool _isFlushing;

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

        public RuleConfiguration RuleConfiguration { get; set; }
        public void ReceivedValue(object val)
        {
            AddValue((T) Convert.ChangeType(val, typeof(T)));
        }

        public void AddValue(T val)
        {
            lock (this)
            {
                CurrentItem = new SingleDataRow<T>(DateTime.UtcNow, val);
                QueuedItems.Add(CurrentItem);
                Flush();
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