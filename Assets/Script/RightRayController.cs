using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RightRayController : MonoBehaviour
{
    public Transform anchor;              // 右手Rayの起点
    private float maxDistance = 100f;     // Rayの最大距離
    private LineRenderer lineRenderer;    // 右手用LineRenderer
    public TargetManager targetManager;   // TargetManagerの参照をインスペクターで設定
    public TargetManager2 targetManager2; // TargetManager2の参照をインスペクターで設定
    public static string ResetSceneName;  // シーン名の保持

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

        // トリガーボタンが押された時にRayのチェックを実行
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            Debug.Log("Trigger pressed");
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
                Debug.Log($"Hit Object: {hitObject.name}");

                // ヒットしたオブジェクトが特定の名前かチェック
                if (hitObject.name == "SceneReset")
                {

                    // 現在のシーンの名前を取得
                    ResetSceneName = SceneManager.GetActiveScene().name;
                    Debug.Log(ResetSceneName);
                    // 削除対象オブジェクトの名前を確認
                    
                    StartRightRayController.removedObjects.Remove("Sphere"+ResetSceneName);
                    

                    // 別のシーンへ遷移
                    SceneManager.LoadScene("Start");
                }
            }
        }
    }
}
