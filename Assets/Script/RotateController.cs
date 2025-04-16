using UnityEngine;

public class RotateController : MonoBehaviour
{
    public Transform controllerTransform; // コントローラのTransformを指定

    // 回転角度
    private Quaternion rotationOffset = Quaternion.Euler(0, 90, 0); // Y軸を90度回転

    void Update()
    {
        if (controllerTransform != null)
        {
            // コントローラの現在の回転にオフセットを適用
            controllerTransform.localRotation = controllerTransform.localRotation * rotationOffset;
        }
    }
}
