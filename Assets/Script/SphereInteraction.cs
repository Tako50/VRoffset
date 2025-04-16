using UnityEngine;
using UnityEngine.Events;

public class SphereInteraction : MonoBehaviour
{
    public UnityEvent onSpherePressed; // スフィアが押された時のイベント

    private void Start()
    {
        if (onSpherePressed == null)
        {
            onSpherePressed = new UnityEvent(); // イベントが設定されていなければ初期化
        }
    }

    void Update()
    {
        // レイキャストの実行
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward); // コントローラーからのレイ



        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Sphere pressed!");
            // ヒットしたオブジェクトがスフィアなら
            if (hit.collider != null && hit.collider.CompareTag("InteractiveSphere"))
            {
                // ボタンが押されたらイベントを発火
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) // トリガーボタンが押された
                {
                    onSpherePressed.Invoke(); // イベントを実行
                    Debug.Log("Sphere pressed!");
                }
            }
        }
       
    }
}
