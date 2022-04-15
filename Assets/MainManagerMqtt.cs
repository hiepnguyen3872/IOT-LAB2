/*
The MIT License (MIT)

Copyright (c) 2018 Giovanni Paolo Vigano'

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

/// <summary>
/// Examples for the M2MQTT library (https://github.com/eclipse/paho.mqtt.m2mqtt),
/// </summary>
namespace MainManagerIOT
{
    public class Status_Data
    {
        public int temperature { get; set; }
        public int humidity { get; set; }
    }

    public class ControlLed_Data
    {
        public string led_status { get; set; }
    }

    public class ControlPump_Data
    {
        public string pump_status { get; set; }
    }

    public class MainManagerMqtt : M2MqttUnityClient
    {
        public List<string> topics = new List<string>();
        public string msg_received_from_topic_status = "";

        private List<string> eventMessages = new List<string>();

        public Status_Data _status_data ;
        public ControlLed_Data _controlLed_data;
        public ControlPump_Data _controlPump_data;

        public void PublishLed()
        {
            _controlLed_data = new ControlLed_Data();
            _controlLed_data = GetComponent<MainManager>().UpdateLed(_controlLed_data);
            string msg_config = JsonConvert.SerializeObject(_controlLed_data);
            client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish led" + msg_config);
        }

        public void PublishPump()
        {
            _controlPump_data = new ControlPump_Data();
            _controlPump_data = GetComponent<MainManager>().UpdatePump(_controlPump_data);
            string msg_config = JsonConvert.SerializeObject(_controlPump_data);
            client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish pump" + msg_config);
        }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }

        protected override void OnConnecting()
        {
            this.brokerAddress = GetComponent<MainManager>().brokerInputField.text;
            this.mqttUserName = GetComponent<MainManager>().userNameInputField.text;
            this.mqttPassword = GetComponent<MainManager>().passwordInputField.text;
            base.OnConnecting();
            Debug.Log("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            GetComponent<MainManager>().SwitchLayer();
            Debug.Log("Connected to broker on " + brokerAddress + "\n");
            GetComponent<MainManager>().UpdateErrorText("");
            // PublishStatus();
            // PublishLed();
            // PublishPump();
            SubscribeTopics();
        }

        protected override void SubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                }
            }
        }

        protected override void UnsubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                }
            }
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.Log("failed connected");
            GetComponent<MainManager>().UpdateErrorText("failed connected");
        }

        protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
        }

        protected override void OnConnectionLost()
        {
            Debug.Log("CONNECTION LOST!");
        }


        protected override void Start()
        {
            base.Start();
        }


        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            if (topic == topics[0])
                ProcessMessageStatus(msg);
        }

        private void ProcessMessageStatus(string msg)
        {
            _status_data = JsonConvert.DeserializeObject<Status_Data>(msg);
            msg_received_from_topic_status = msg;
            Debug.Log(_status_data.temperature);
            GetComponent<MainManager>().UpdateUI(_status_data);
            // GetComponent<MainManager>().Update_Status(_status_data);
        }

        private void StoreMessage(string eventMsg)
        {
            eventMessages.Add(eventMsg);
        }

        private void ProcessMessage(string msg)
        {
            Debug.Log("Received: " + msg);
        }

        protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()

            if (eventMessages.Count > 0)
            {
                foreach (string msg in eventMessages)
                {
                    ProcessMessage(msg);
                }
                eventMessages.Clear();
            }
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {
            // if (autoTest)
            // {
            //     autoConnect = true;
            // }
        }
    }
}
