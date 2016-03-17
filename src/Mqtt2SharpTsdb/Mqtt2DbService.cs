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
using System.IO;
using System.Reflection;
using System.Text;
using log4net;
using Mqtt2SharpTsdb.Config;
using Mqtt2SharpTsdb.Items;
using Mqtt2SharpTsdb.Rules;
using Nancy.Json;
using Newtonsoft.Json;
using SharpTsdbClient;
using Timeenator.Impl;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Mqtt2SharpTsdb
{
    public class Mqtt2DbService
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly MqttClient _client = new MqttClient("10.10.1.77");
        private readonly string _dbName = "Haus";
        private DbClient _dbClient;
        private Dictionary<string, MqttItem> _mqttItems = new Dictionary<string, MqttItem>();
        private Dictionary<string, MeasurementItem> _measurementItems = new Dictionary<string, MeasurementItem>();
        private RuleConfiguration _ruleConfiguration;

        public async void Init()
        {
            LoadConfig();
            _dbClient = new DbClient(new Client("10.10.1.77"), _dbName);
            await _dbClient.CreateOrAtachDbAsync();
            _client.Connect("Mqtt2SharpTsdb");
            _client.MqttMsgPublishReceived += ClientOnMqttMsgPublishReceived;
            _client.Subscribe(new[] {"#"}, new byte[] {0});
            Logger.Info("Initialized");
        }

        private void LoadConfig()
        {
            var filePath = ConfigFilePath();
            if (File.Exists(filePath))
            {
                _ruleConfiguration = new JavaScriptSerializer(null, false, Int32.MaxValue, Int32.MaxValue, true, false).Deserialize<RuleConfiguration>(File.ReadAllText(filePath));
            }
            else
            {
                LoadDefaultConfig();
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            var filePath = ConfigFilePath();
            File.WriteAllText(filePath, JsonConvert.SerializeObject(_ruleConfiguration, Formatting.Indented));
        }

        private void LoadDefaultConfig()
        {
            _ruleConfiguration = new RuleConfiguration();
            _ruleConfiguration.IgnoringRules.Add(new IgnoreRule("^OpenHAB.out.cb"));
            _ruleConfiguration.NamingRules.Add(new NamingRule("/", "."));
            _ruleConfiguration.TypeRules.Add(new TypeRule(".State$", "byte"));
            _ruleConfiguration.TypeRules.Add(new TypeRule(".State$", "byte"));
            _ruleConfiguration.TextConverterRules.Add(new TextConverterRule("ON", 1));
            _ruleConfiguration.TextConverterRules.Add(new TextConverterRule("OFF", 0));
            _ruleConfiguration.TextConverterRules.Add(new TextConverterRule("OPEN", 1));
            _ruleConfiguration.TextConverterRules.Add(new TextConverterRule("CLOSED", 0));
            _ruleConfiguration.RecordingRules.Add(new RecordingRule("State$", "change"));
            _ruleConfiguration.RecordingRules.Add(new RecordingRule(".*", "30s"));
        }

        private static string ConfigFilePath()
        {
            var fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var dir = fileInfo.Directory;
            return Path.Combine(dir.FullName, "config.json");
        }

        private async void ClientOnMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs mqttMsgPublishEventArgs)
        {
            try
            {
                var message = Encoding.UTF8.GetString(mqttMsgPublishEventArgs.Message);
                Logger.Debug($"Got {mqttMsgPublishEventArgs.Topic}, content: {message}");
                try
                {
                    float val;
                    if (message == "OFF")
                    {
                        val = 0;
                    }
                    else if (message == "ON")
                    {
                        val = 1;
                    }
                    else
                    {
                        val = float.Parse(message);
                    }

                    await _dbClient.Measurement(mqttMsgPublishEventArgs.Topic.Replace("/", "."))
                        .AppendAsync(new[] { new SingleDataRow<float>(DateTime.Now, val) }, false);
                }
                catch (Exception ex)
                {
                    Logger.Warn(
                        $"Excpetion in topic {mqttMsgPublishEventArgs.Topic}, content: {message}, reason: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Catched Exception: {ex.Message}");
            }
           
        }

        public void Stop()
        {
            _client.Disconnect();
        }
    }
}