print("IOT GATEWAY")
import paho.mqtt.client as mqttclient
import json
import time
BROKER_ADDRESS = "mqttserver.tk"
PORT = 1883
USERNAME = "bkiot"
PASSWORD = "12345678"


def subscribed(client, userdata, mid, granted_qos):
    print("Subscribed...")


def connected(client, usedata, flags, rc):
    if rc == 0:
        print("Thingsboard connected successfully!!")
        client.subscribe("/bkiot/1913396/status")
    else:
        print("Connection is failed")


client = mqttclient.Client("Gateway_Thingsboard")
client.username_pw_set(USERNAME, PASSWORD)

client.on_connect = connected
client.connect(BROKER_ADDRESS, 1883)
client.loop_start()

client.on_subscribe = subscribed
temp = 30
humi = 50
while True:
    temp += 1
    humi += 1
    status_data = {'temperature': temp, 'humidity': humi}
    try:
        client.publish('/bkiot/1913396/status', json.dumps(status_data))
    except:
        pass
    time.sleep(10)