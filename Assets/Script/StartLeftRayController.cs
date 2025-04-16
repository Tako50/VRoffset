using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLeftRayController : MonoBehaviour
{
    public Transform anchor;              // 左手Rayの起点
    private float maxDistance = 100;      // Rayの最大距離
    private LineRenderer lineRenderer;    // 左手用LineRenderer
    private const float triggerThreshold = 0.8f;
    private bool isTriggerPressed = false; // トリガーが押されているかの状態
    public StartRightRayController StartRightRayController ;   // TargetManagerの参照をインスペクターで設定

    // 削除対象オブジェクトのリスト（右手用と共有）

    void Start()
    {
        // 左手用LineRendererの取得
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;

        // シーン内の削除対象オブジェクトを削除
        RemoveMarkedObjects();
    }

    void Update()
    {
        // Rayを飛ばす処理
        ShootRay();

        // トリガーが押されていない場合にリセット
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 0)
        {
            ResetTriggerState();
        }

        // トリガーが押された場合に成功判定を実行
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > triggerThreshold && !isTriggerPressed)
        {
            isTriggerPressed = true; // トリガーを押した状態に更新
        }
    }

    // RayのヒットをTargetManagerに渡す
   

    // 左手Rayを飛ばしてその可視化を行う
    void ShootRay()
    {
        RaycastHit hit;
        Ray ray = new Ray(anchor.position, anchor.forward);

        // 左手Rayの可視化
        lineRenderer.SetPosition(0, ray.origin);
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1, ray.origin + (ray.direction * maxDistance));
        }
        StartRightRayController.Ray(hit, ray.direction, anchor.position);
    }

    // トリガー状態をリセット
    void ResetTriggerState()
    {
        isTriggerPressed = false; // トリガー押下状態をリセット
    }

    // シーン読み込み時に削除対象のオブジェクトを削除
    void RemoveMarkedObjects()
    {
        foreach (string objectName in StartRightRayController.removedObjects)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }
    // オブジェクトの色を変更するメソッド
    void ChangeObjectColor(GameObject obj, Color color)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
        else
        {
            Debug.LogWarning($"GameObject {obj.name} に MeshRenderer が見つかりません。");
        }
    }

}
