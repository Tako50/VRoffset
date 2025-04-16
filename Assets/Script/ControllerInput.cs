using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Linq;

public class ControllerInput : MonoBehaviour
{
    private InputDevice leftController;
    private InputDevice rightController;

    void Start()
    {
        // デバイスリストを取得
        var devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);

        // デバイスが空でないか確認
        if (devices.Count == 0)
        {
            Debug.LogWarning("No devices found.");
        }
        else
        {
            // すべての接続されたデバイスのログを表示
            foreach (var device in devices)
            {
                Debug.Log($"Device found: {device.name}, Characteristics: {device.characteristics}");
            }
        }

        // 左右のコントローラーデバイスをセット
        var leftHandCharacteristics = InputDeviceCharacteristics.Left;
        var rightHandCharacteristics = InputDeviceCharacteristics.Right;

        leftController = devices.FirstOrDefault(d => d.characteristics.HasFlag(leftHandCharacteristics));
        rightController = devices.FirstOrDefault(d => d.characteristics.HasFlag(rightHandCharacteristics));

        Debug.Log("Device checking complete.");

        // 初期コントローラーの接続確認
        CheckControllerConnection();
    }

    void Update()
    {
        // コントローラーが無効であれば再確認
        if (!leftController.isValid || !rightController.isValid)
        {
            var devices = new List<InputDevice>();
            InputDevices.GetDevices(devices);

            if (devices.Count == 0)
            {
                Debug.LogWarning("No devices found.");
            }
            else
            {
                leftController = devices.FirstOrDefault(d => d.characteristics.HasFlag(InputDeviceCharacteristics.Left));
                rightController = devices.FirstOrDefault(d => d.characteristics.HasFlag(InputDeviceCharacteristics.Right));

                CheckControllerConnection();
            }
        }
    }

    // コントローラーの接続確認
    private void CheckControllerConnection()
    {
        if (leftController.isValid)
        {
            Debug.Log("Left controller is connected.");
        }
        else
        {
            Debug.Log("Left controller is not connected.");
        }

        if (rightController.isValid)
        {
            Debug.Log("Right controller is connected.");
        }
        else
        {
            Debug.Log("Right controller is not connected.");
        }
    }
}
