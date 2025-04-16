using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartRightRayController : MonoBehaviour
{
    public Transform anchor;              // 右手Rayの起点
    private float maxDistance = 100;      // Rayの最大距離
    private LineRenderer lineRenderer;    // 右手用LineRenderer
    private const float triggerThreshold = 0.8f;
    private bool isTriggerPressed = false; // トリガーが押されているかの状態
    private RaycastHit leftHit;
    private Vector3 leftRayDirection;
    private Vector3 leftAnchorPosition;

    // 削除対象オブジェクトのリスト（シーン間で永続化される）
    public static List<string> removedObjects = new List<string>();


    public void Ray(RaycastHit hit, Vector3 rayDirection, Vector3 anchorPosition)
    {
        // 値を更新
        leftHit = hit;
        leftRayDirection = rayDirection;
        leftAnchorPosition = anchorPosition;

    }


    void Start()
    {
        // 右手用LineRendererの取得
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;


        foreach (string objectName in removedObjects)
        {
            Debug.Log($"削除対象オブジェクトの名前: {objectName}");
            GameObject obj = GameObject.Find(objectName);

            if (obj != null)
            {
                Debug.Log($"{objectName} がシーン内に存在します。");
            }
            else
            {
                Debug.LogWarning($"{objectName} がシーン内に見つかりません。");
            }
        }
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
            PerformRayCheck();
            isTriggerPressed = true; // トリガーを押した状態に更新
        }
    }

    // RayのヒットをTargetManagerに渡す
    void PerformRayCheck()
    {
        RaycastHit hit;
        Ray ray = new Ray(anchor.position, anchor.forward);
        GameObject trainingObject = GameObject.Find("Training");
        GameObject testObject = GameObject.Find("Test");

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;
                GameObject lefthitObject = leftHit.collider.gameObject;
                Debug.Log(hitObject);
                Debug.Log(lefthitObject);
                // ヒットしたオブジェクトが青いオブジェクトと一致するかチェック
                if (hitObject.name == "Training" && lefthitObject.name == "Training")
                {
                    ModeManager.mode = false;  // ModeManagerで共通のmodeを変更
                    ChangeObjectColor(hitObject, Color.red); // Training の場合は赤
                    ChangeObjectColor(testObject, Color.white);
                    Debug.Log(ModeManager.mode);
                }
                else if (hitObject.name == "Test" && lefthitObject.name == "Test")
                {
                    ModeManager.mode = true;  // ModeManagerで共通のmodeを変更
                    ChangeObjectColor(hitObject, Color.blue); // Test の場合は青
                    ChangeObjectColor(trainingObject, Color.white);
                    Debug.Log(ModeManager.mode);

                }
                else if (hitObject.name.StartsWith("Sphere") && lefthitObject.name.StartsWith("Sphere"))
                {
                    if (hitObject.name.Replace("Sphere", "") == lefthitObject.name.Replace("Sphere", ""))
                    {
                        // シーンをロードする前に、オブジェクト名を削除リストに追加
                        if (ModeManager.mode)
                        {
                            if (!removedObjects.Contains(hitObject.name))
                            {
                                removedObjects.Add(hitObject.name);
                            }
                        }

                        // シーン遷移
                        SceneManager.LoadScene(hitObject.name.Replace("Sphere", ""));
                    }
                }
            }
        }
    }

    // 右手Rayを飛ばしてその可視化を行う
    void ShootRay()
    {
        RaycastHit hit;
        Ray ray = new Ray(anchor.position, anchor.forward);

        // 右手Rayの可視化
        lineRenderer.SetPosition(0, ray.origin);
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1, ray.origin + (ray.direction * maxDistance));
        }
    }

    // トリガー状態をリセット
    void ResetTriggerState()
    {
        isTriggerPressed = false; // トリガー押下状態をリセット
    }

    // シーン読み込み時に削除対象のオブジェクトを削除
    void RemoveMarkedObjects()
    {
        foreach (string objectName in removedObjects)
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
