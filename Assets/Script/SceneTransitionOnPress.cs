using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using System.Linq;


public class ScenTransitionOnPress : MonoBehaviour
{
    private List<string> sphereNames = new List<string>();
    private List<string> sphereNames1 = new List<string>();
    private int j = 0; // インデックス用の変数

    private GameObject lastChangedObject = null; // 最後に色を変更したオブジェクトを保持
    private const float triggerThreshold = 0.8f;
    private bool isTriggerPressed = false;
    private Dictionary<string, Vector3> spherePositions = new Dictionary<string, Vector3>();
    // 名前と角度（オイラー角）を保存するDictionary
    private Dictionary<string, Vector3> sphereAngles = new Dictionary<string, Vector3>();
    public List<bool> successFlags = new List<bool>(); // 成功フラグのリスト
    private List<float> xPositions = new List<float>();
    private List<float> yPositions = new List<float>();
    private List<float> zPositions = new List<float>();
    private Vector3 objectRotation;
    private List<float> xAnchorPositions = new List<float>();
    private List<float> yAnchorPositions = new List<float>();
    private List<float> zAnchorPositions = new List<float>();
    public static bool isTaskActive = false; // タスクがアクティブかどうか（static変数）

    private float startTime;
    

    public string GetCurrentBlueObjectName()
    {
        if (j - 1 < sphereNames.Count && j - 1 >= 0) // 現在のインデックス範囲内を確認
        {
            return sphereNames[j - 1];
        }
        return null;
    }


    void Start()
    {
        
    }

    void Update()
    {

    }
    public void StartTask()
    {
        isTaskActive = true; // タスクを開始
        Debug.Log("Task Started!");
    }

    // 次のオブジェクトの色を変更し、前回のオブジェクトの色をリセットする
    void ChangeColorToNextObject()
    {
        // トリガーが押されていない場合にのみ次のオブジェクトを変更
        if (!isTriggerPressed)
        {
            // 前回のオブジェクトの色をリセット
            ResetAllObjectsColor();  // すべてのオブジェクトの色を白にリセット

            // 次のオブジェクトの色を変更
            if (j < sphereNames.Count) // jがリスト範囲内か確認
            {
                SetInitialBlueObject();
                // データをCSVファイルに書き込む処理を追加
                WriteDataToCSV();

                Debug.Log(j);
            }
            else
            {
                float elapsedTime = Time.time - startTime;
                Debug.Log("Object destroyed in " + elapsedTime + " seconds.");
                Debug.Log("end");
            }
        }
    }

    // 最初の青いオブジェクトを設定する処理
    void SetInitialBlueObject()
    {
        GameObject targetObject = GameObject.Find(sphereNames[j]);
        if (targetObject != null)
        {
            MeshRenderer renderer = targetObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.blue; // 最初のオブジェクトの色を青に変更
            }
            lastChangedObject = targetObject; // 最後に変更したオブジェクトを保存
        }
        j++;
    }

    // トリガー状態をリセット
    void ResetTriggerState()
    {
        isTriggerPressed = false;
    }

    // シャッフル処理
    void ShuffleListMultipleTimesAndCombine(List<string> list, string lastObject, int times = 2)
    {
        List<string> combinedList = new List<string>();

        for (int i = 0; i < times; i++)
        {
            ShuffleList(list, lastObject);
            combinedList.AddRange(list);
        }
        sphereNames = combinedList;
    }

    // リストのシャッフル処理
    void ShuffleList(List<string> list, string lastObject)
    {
        System.Random rand = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            string value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        // 最初のオブジェクトがlastObjectと同じ場合、再度シャッフル
        if (list[0] == lastObject)
        {
            ShuffleList(list, lastObject);
        }
    }

    // すべてのオブジェクトの色をリセット
    void ResetAllObjectsColor()
    {
        foreach (string sphereName in sphereNames1)
        {
            GameObject targetObject = GameObject.Find(sphereName);
            if (targetObject != null)
            {
                MeshRenderer renderer = targetObject.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.white; // 色を白に変更
                }
            }
        }
    }
    // 青いオブジェクトへのヒットを確認する関数
    public void CheckRayHit(RaycastHit hit)
    {
        // 現在青いオブジェクト名を取得
        string currentBlueObjectName = GetCurrentBlueObjectName();
        Debug.Log(currentBlueObjectName);
        // 指定した名前のオブジェクトを取得
        GameObject blueObject = GameObject.Find(currentBlueObjectName);
        if (blueObject != null)
        {
            // オブジェクトのポジションを取得
            Vector3 position = blueObject.transform.position;
        }


        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;

            // ヒットしたオブジェクトが青いオブジェクトと一致するかチェック
            if (hitObject.name == currentBlueObjectName)
            {
                Debug.Log("success: " + hitObject.name);
                successFlags.Add(true);
                Vector3 position = sphereAngles[currentBlueObjectName];
                xPositions.Add(position.x);
                yPositions.Add(position.y);
                zPositions.Add(position.z);
                Vector3 objectRotation = transform.rotation.eulerAngles;  // ここでtransform.rotation.eulerAnglesを使う
                xAnchorPositions.Add(objectRotation.x);
                yAnchorPositions.Add(objectRotation.y);
                zAnchorPositions.Add(objectRotation.z);

            }
            else if (hitObject.name == "SphereButton"){
                Destroy(hitObject);
                StartTask();
                startTime = Time.time;
                
            }
            else
            {
                successFlags.Add(false);
                Vector3 position = sphereAngles[currentBlueObjectName];
                xPositions.Add(position.x);
                yPositions.Add(position.y);
                zPositions.Add(position.z);
                Vector3 objectRotation = transform.rotation.eulerAngles;
                xAnchorPositions.Add(objectRotation.x);
                yAnchorPositions.Add(objectRotation.y);
                zAnchorPositions.Add(objectRotation.z);

            }
        }
        else
        {
            successFlags.Add(false);
            Vector3 position = sphereAngles[currentBlueObjectName];
            xPositions.Add(position.x);
            yPositions.Add(position.y);
            zPositions.Add(position.z);
            Vector3 objectRotation = transform.rotation.eulerAngles;
            xAnchorPositions.Add(objectRotation.x);
            yAnchorPositions.Add(objectRotation.y);
            zAnchorPositions.Add(objectRotation.z);

        }
    }


    void WriteDataToCSV()
    {
        
    }

}
