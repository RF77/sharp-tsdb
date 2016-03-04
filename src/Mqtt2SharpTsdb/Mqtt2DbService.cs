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
using System.Reflection;
using System.Text;
using log4net;
using SharpTsdbClient;
using Timeenator.Impl;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Mqtt2SharpTsdb
{
    public class Mqtt2DbService
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly MqttClient _client = new MqttClient("localhost");
        private readonly string _dbName = "Haus";
        private DbClient _dbClient;

        public async void Init()
        {
            _dbClient = new DbClient(new Client("localhost"), _dbName);
            await _dbClient.CreateOrAtachDbAsync();
            _client.Connect("Mqtt2SharpTsdb");
            _client.MqttMsgPublishReceived += ClientOnMqttMsgPublishReceived;
            _client.Subscribe(new[] {"#"}, new byte[] {0});
            Logger.Info("Initialized");
        }

        private async void ClientOnMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs mqttMsgPublishEventArgs)
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
                    .AppendAsync(new[] {new SingleDataRow<float>(DateTime.Now, val)}, false);
            }
            catch (Exception ex)
            {
                Logger.Warn(
                    $"Excpetion in topic {mqttMsgPublishEventArgs.Topic}, content: {message}, reason: {ex.Message}");
            }
        }

        public void Stop()
        {
            _client.Disconnect();
        }
    }
}