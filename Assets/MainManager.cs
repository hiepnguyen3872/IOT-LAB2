using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MainManagerIOT
{
    public class MainManager : MonoBehaviour
    {
        public CanvasGroup _canvasLayer1;
        public CanvasGroup _canvasLayer2;
        public InputField brokerInputField;
        public InputField userNameInputField;
        public InputField passwordInputField;
        public Text temp_data;
        public Text humi_data;
        public Toggle led;
        public Toggle pump;
        public Button connectButton;
        public Text errorMessage;
        private Tween twenFade;

        public void Start()
        {
            this.brokerInputField.text = "mqttserver.tk";
            this.userNameInputField.text = "bkiot";
            this.passwordInputField.contentType = InputField.ContentType.Password;
            this.passwordInputField.text = "12345678";
        }


        public void Fade(CanvasGroup _canvas, float endValue, float duration, TweenCallback onFinish)
        {
            if (twenFade != null)
            {
                twenFade.Kill(false);
            }

            twenFade = _canvas.DOFade(endValue, duration);
            twenFade.onComplete += onFinish;
        }

        public void FadeIn(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 1f, duration, () =>
            {
                _canvas.interactable = true;
                _canvas.blocksRaycasts = true;
            });
        }

        public void FadeOut(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 0f, duration, () =>
            {
                _canvas.interactable = false;
                _canvas.blocksRaycasts = false;
            });
        }



        IEnumerator _IESwitchLayer()
        {
            if (_canvasLayer1.interactable == true)
            {
                FadeOut(_canvasLayer1, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer2, 0.25f);
            }
            else
            {
                FadeOut(_canvasLayer2, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer1, 0.25f);
            }
        }

        public void SwitchLayer()
        {
            StartCoroutine(_IESwitchLayer());
        }

        public void UpdateUI(Status_Data _status_data)
        {
            this.temp_data.text = _status_data.temperature + "°C";
            this.humi_data.text = _status_data.humidity + "°C";
            // foreach(data_ss _data in _status_data.data_ss)
            // {
            //     switch (_data.ss_name)
            //     {

            //         case "temperature":
            //             this.temp_data.text = _data.ss_value + "°C";
            //             break;

            //         case "humidity":
            //             this.humi_data.text = _data.ss_value + "°C";
            //             break;
            //         //case "device_status":
            //         //    Debug.Log("_data.ss_value " + _data.ss_value);
            //         //    if (_data.ss_value == "1")
            //         //        _btn_config.interactable = true;

            //         //    break;
            //     }

            // }

        }

        public ControlLed_Data UpdateLed(ControlLed_Data _controlLed_data)
        {
            if (led.isOn)
                _controlLed_data.led_status = "On";
            else
                _controlLed_data.led_status = "Off";
            return _controlLed_data;
        }

        public ControlPump_Data UpdatePump(ControlPump_Data _controlPump_data)
        {
            Debug.Log("pump pump pump");
            if (pump.isOn)
                _controlPump_data.pump_status = "On";
            else
                _controlPump_data.pump_status = "Off";
            return _controlPump_data;
        }

        public void UpdateErrorText(string error_text)
        {
            this.errorMessage.text = error_text;
        }

    }
}