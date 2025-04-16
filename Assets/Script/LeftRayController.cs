using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeftRayController : MonoBehaviour
{
    public Transform anchor;              // 左手Rayの起点
    private float maxDistance = 100f;     // Rayの最大距離
    private LineRenderer lineRenderer;    // 左手用LineRenderer
    public TargetManager targetManager;   // TargetManagerの参照をインスペクターで設定
    public TargetManager2 targetManager2; // TargetManager2の参照をインスペクターで設定
    public static string ResetSceneName;  // シーン名の保持
    private const float triggerThreshold = 0.8f;
    private const string SceneResetObjectName = "SceneReset"; // SceneResetの名前を定数で管理

    void Start()
    {
        // LineRendererの取得と設定
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
    }

    void Update()
    {
        // Rayを飛ばしてヒット情報を更新し、TargetManagerに渡す
        ShootRay();

        // 左手トリガーボタンが押された時にRayのチェックを実行
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) // 左手のトリガー
        {
            Debug.Log("Left Trigger pressed");
            PerformRayCheck();
        }
    }

    void ShootRay()
    {
        RaycastHit hit;
        Ray ray = new Ray(anchor.position, anchor.forward);

        // Rayの可視化
        lineRenderer.SetPosition(0, ray.origin);
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1, ray.origin + (ray.direction * maxDistance));
            hit = default; // 当たらなかった場合
        }

        // TargetManagerにRayの結果を渡す
        targetManager.Ray(hit, ray.direction, anchor.position);
        targetManager2.Ray(hit, ray.direction, anchor.position);
    }

    void PerformRayCheck()
    {
        RaycastHit hit;
        Ray ray = new Ray(anchor.position, anchor.forward);

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;
                Debug.Log($"Left Hand Hit Object: {hitObject.name}");

                // ヒットしたオブジェクトが特定の名前かチェック
                if (hitObject.name == SceneResetObjectName)
                {
                    // 現在のシーンの名前を取得
                    ResetSceneName = SceneManager.GetActiveScene().name;

                    StartRightRayController.removedObjects.Remove("Sphere"+ResetSceneName);
                
    
                    // 別のシーンへ遷移
                    SceneManager.LoadScene("Start");
                }
            }
        }
    }
}
