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
using System.Threading.Tasks;
using log4net;
using Mqtt2SharpTsdb.Config;
using Mqtt2SharpTsdb.Items;
using Mqtt2SharpTsdb.Rules;
using Nancy.Json;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Mqtt2SharpTsdb
{
    public class Mqtt2DbService
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly MqttClient _client = new MqttClient(_brokerHostName);
        private readonly Dictionary<string, MqttItem> _mqttItems = new Dictionary<string, MqttItem>();
        private RuleConfiguration _ruleConfiguration;
        private static string _brokerHostName = "10.10.1.20";

        public async void Init()
        {
            LoadConfig();
            await ReconnectAsync();  
            _client.MqttMsgPublishReceived += ClientOnMqttMsgPublishReceived;
            _client.ConnectionClosed += _client_ConnectionClosed;
            _client.Subscribe(new[] {"#"}, new byte[] {0});
            Logger.Info("Initialized");
        }

        private void ConnectMqtt()
        {
            Logger.Debug($"Trying to connect to MQTT Server {_brokerHostName}");
            //_client.Connect("Mqtt2SharpTsdb", null, null, false, (byte)1, true, "Device/Mqtt2SharpTsdb/State", "-1", false, 30);
            _client.Connect("Mqtt2SharpTsdb");
            Logger.Info($"Connected to MQTT broker {_brokerHostName}");
        }

        private async void _client_ConnectionClosed(object sender, EventArgs e)
        {
            Logger.Info("Mqtt Connection closed -> reconnect soon...");
            await ReconnectAsync();
        }

        private async Task ReconnectAsync()
        {
            try
            {
                await Task.Delay(1000);
                ConnectMqtt();
            }
            catch (Exception ex)
            {
                Logger.Error($"Couldn't connect to MQTT broker, try again..., reason:{ex.Message} ");
                await ReconnectAsync();
            }
        }

        private void LoadConfig()
        {
            var filePath = ConfigFilePath();
            if (File.Exists(filePath))
            {
                _ruleConfiguration = new JavaScriptSerializer(null, false, int.MaxValue, int.MaxValue, true, false).Deserialize<RuleConfiguration>(File.ReadAllText(filePath));
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
            _ruleConfiguration.TextConverterRules.Add(new TextConverterRule("ON", "1"));
            _ruleConfiguration.TextConverterRules.Add(new TextConverterRule("OFF", "0"));
            _ruleConfiguration.TextConverterRules.Add(new TextConverterRule("OPEN", "1"));
            _ruleConfiguration.TextConverterRules.Add(new TextConverterRule("CLOSED", "0"));
            _ruleConfiguration.JsonConverterRules.Add(new JsonConverterRule("^hm/status/.*", "val"));
            _ruleConfiguration.RecordingRules.Add(new RecordingRule("testChange", "change"));
            _ruleConfiguration.RecordingRules.Add(new RecordingRule("testInterval", "10s"));
            _ruleConfiguration.RecordingRules.Add(new RecordingRule("State$", "change"));
            _ruleConfiguration.RecordingRules.Add(new RecordingRule(".*", "30s"));
        }

        private static string ConfigFilePath()
        {
            var fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var dir = fileInfo.Directory;
            // ReSharper disable once PossibleNullReferenceException
            return Path.Combine(dir.FullName, "config.json");
        }

        private void ClientOnMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs mqttMsgPublishEventArgs)
        {
            try
            {
                var message = Encoding.UTF8.GetString(mqttMsgPublishEventArgs.Message);
                Logger.Debug($"Got {mqttMsgPublishEventArgs.Topic}, content: {message}");

                MqttItem item;
                if (!_mqttItems.TryGetValue(mqttMsgPublishEventArgs.Topic, out item))
                {
                    item = new MqttItem(mqttMsgPublishEventArgs.Topic, _ruleConfiguration);
                    _mqttItems[mqttMsgPublishEventArgs.Topic] = item;
                }
                item.ReceivedMessage(message, mqttMsgPublishEventArgs);
               
            }
            catch (Exception ex)
            {
                Logger.Error($"Catched Exception for topic \"{mqttMsgPublishEventArgs.Topic}\": {ex.Message}");
            }
           
        }

        public void Stop()
        {
            _client.Disconnect();
        }
    }
}