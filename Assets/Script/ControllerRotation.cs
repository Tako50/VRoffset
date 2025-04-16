using UnityEngine;
using Meta;

public class ControllerRotationWithMeta : MonoBehaviour
{
    public OVRInput.Controller controller;  // OVRInputでコントローラを指定
    public float rotationAngle = 90f;  // 90度回転させる

    void Update()
    {
        // コントローラの回転を取得
        Quaternion rotation = OVRInput.GetLocalControllerRotation(controller);

        // 回転に90度を加算
        rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y + rotationAngle, rotation.eulerAngles.z);

        // 回転を反映
        transform.rotation = rotation;
    }
}
