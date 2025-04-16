using UnityEngine;

public class VRControllerAdjuster : MonoBehaviour
{
    public Transform leftHandAnchor;  // 左手のアンカー
    public Transform rightHandAnchor; // 右手のアンカー
    public Transform leftControllerAnchor; // 左コントローラーのアンカー
    public Transform rightControllerAnchor; // 右コントローラーのアンカー

    public OVRInput.Controller leftActiveController = OVRInput.Controller.LTouch;  // 左コントローラー
    public OVRInput.Controller rightActiveController = OVRInput.Controller.RTouch; // 右コントローラー

    private Quaternion rotationOffset = Quaternion.Euler(-90f, 0f, 0f); // 回転オフセット

    void LateUpdate()
    {
        // コントローラーのトラッキング情報を取得してアンカーに適用
        leftHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(leftActiveController);
        rightHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(rightActiveController);

        leftHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(leftActiveController);
        rightHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(rightActiveController);

        Debug.Log("Before applying rotationOffset:");
        Debug.Log($"LeftHand Rotation: {leftHandAnchor.localRotation}");

        // 回転オフセットを適用
        leftHandAnchor.localRotation *= rotationOffset;
        rightHandAnchor.localRotation *= rotationOffset;

    }
}
