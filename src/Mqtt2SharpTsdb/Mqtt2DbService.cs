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
        private MqttClient _client = new MqttClient("localhost");
        private DbClient _dbClient;
        private string _dbName = "Haus";

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
                    .AppendAsync(new[] { new SingleDataRow<float>(DateTime.Now, val) }, false);

            }
            catch (Exception ex)
            {
                Logger.Warn($"Excpetion in topic {mqttMsgPublishEventArgs.Topic}, content: {message}, reason: {ex.Message}");
            }
        }

        public void Stop()
        {
            _client.Disconnect();
        }
    }
}
