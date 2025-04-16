using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;




public class TargetManager2 : MonoBehaviour
{
    private List<string> sphereNames = new List<string>();
    private List<string> sphereNames1 = new List<string>();
    private int j = 0; // インデックス用の変数

    public int myintJ
    {
        get { return j; }
        set { j = value; }
    }

    private GameObject lastChangedObject = null; // 最後に色を変更したオブジェクトを保持
    private const float triggerThreshold = 0.8f;
    private bool isTriggerPressed = true;
    private string successFlag; // 成功か失敗かを記録する

    private Dictionary<string, Vector3> spherePositions = new Dictionary<string, Vector3>();
    // 名前と角度（オイラー角）を保存するDictionary

    public static bool isTaskActive = false; // タスクがアクティブかどうか（static変数）

    private float LastTime = 0;
    private float SelectTime = 0;
    private float TaskTime;

    public Scene currentScene;
    private RaycastHit currentHit;
    private Vector3 currentRayDirection;
    private Vector3 currentAnchorPosition;
    private int Count = 0;

    private string lastobject = string.Empty;

    private GameObject lastHitObject; // 前回のRayが当たったオブジェクト
    private Color originalColor; // 元の色を保持



    public AudioSource audioSource; // 音を再生するAudioSource
    public AudioClip successSound; // 成功時に鳴らす音
    public AudioClip failureSound; // 失敗時に鳴らす音
    List<string> dataList = new List<string>(); // グローバルにリストを宣言
    public string filename = "DebugData_19.csv";

    public void StartTask()
    {
        isTaskActive = true; // タスクを開始
        j = 0;  // タスク再開時にインデックスをリセット
    }
    public void Ray(RaycastHit hit, Vector3 rayDirection, Vector3 anchorPosition)
    {
        // 値を更新
        currentHit = hit;
        currentRayDirection = rayDirection;
        currentAnchorPosition = anchorPosition;

    }


    void Start()
    {
        // AudioSourceの設定
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); // オブジェクトにAudioSourceが無い場合は、コンポーネントを取得
        }
        // 最初はタスクを停止状態にする

        isTaskActive = false;
        // Sphere1〜Sphere8の名前をリストに追加
        for (int i = 1; i <= 8; i++)
        {
            string sphereName = "Sphere" + i;
            sphereNames.Add(sphereName);
            GameObject sphereObject = GameObject.Find(sphereName);

            if (sphereObject != null)
            {
                float precision = 1E-10f;  // 小数点以下5桁で丸め
                // 名前と位置を保持
                Vector3 roundedPosition = new Vector3
                (

                    Mathf.Round(sphereObject.transform.position.x / precision) * precision,
                    Mathf.Round(sphereObject.transform.position.y / precision) * precision,
                    Mathf.Round(sphereObject.transform.position.z / precision) * precision
                );
                spherePositions[sphereName] = roundedPosition;
            }
        }
        sphereNames1 = new List<string>(sphereNames); // 最初のリストのコピー

        MultipleTimesAndCombine(sphereNames);
        SetCenterBlueObject();
    }

    void MultipleTimesAndCombine(List<string> list)
    {

        int times = ModeManager.mode ? 20 : 150; // modeがtrueなら15回、falseなら1回
        List<string> combinedList = new List<string>();

        for (int i = 0; i < times; i++)
        {
            combinedList.AddRange(list);
        }
        sphereNames = combinedList;
    }


    void Update()
    {

        float leftTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float rightTriggerValue = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);


        // トリガーが引かれていない状態
        if (leftTriggerValue < triggerThreshold && rightTriggerValue < triggerThreshold)
        {
            if (currentHit.collider != null) // レイが何かにヒットしている場合
            {
                GameObject hitObject = currentHit.collider.gameObject;
                MeshRenderer renderer = currentHit.collider.GetComponent<MeshRenderer>();

                if (renderer != null)
                {
                    // 現在ヒットしているオブジェクトが前回と異なる場合のみ処理を行う
                    if (lastHitObject != hitObject)
                    {
                        // 以前のオブジェクトがあればその色を元に戻す
                        if (lastHitObject != null)
                        {
                            MeshRenderer lastRenderer = lastHitObject.GetComponent<MeshRenderer>();
                            if (lastRenderer != null)
                            {
                                lastRenderer.material.color = originalColor; // 元の色に戻す
                            }
                        }

                        // 新しいオブジェクトの色を記録して変更
                        originalColor = renderer.material.color; // 元の色を保持
                        renderer.material.color = Color.green;   // オブジェクトを緑色に変更

                    }
                }

                lastHitObject = hitObject; // 現在のオブジェクトを記録
            }
            else if (lastHitObject != null) // ヒットしているオブジェクトがなく、前回のオブジェクトがある場合
            {
                MeshRenderer renderer = lastHitObject.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material.color = originalColor; // 元の色に戻す
                }

                lastHitObject = null; // 追跡をリセット
            }
        }
        else // トリガーが引かれている場合
        {
            // トリガーが引かれている間、前回ヒットしたオブジェクトの色を元に戻す
            if (lastHitObject != null)
            {
                MeshRenderer renderer = lastHitObject.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material.color = originalColor; // 元の色に戻す
                }

                lastHitObject = null; // 追跡をリセット
            }
        }







        if ((rightTriggerValue > triggerThreshold || leftTriggerValue > triggerThreshold) && currentHit.collider != null && currentHit.collider.name == "SphereButton" && Count == 0)
        {
            // "SphereButton"の位置を取得
            //Vector3 buttonPosition = currentHit.collider.transform.position;
            Vector3 buttonPosition = new Vector3(0, 0, 5);

            // 名前を指定してspherePositionsに追加
            string sphereButtonName = "SphereButton"; // 任意で変更可能
            spherePositions[sphereButtonName] = buttonPosition;
            GameObject sphereButton = GameObject.Find(sphereButtonName);
            if (sphereButton != null)
            {
                Destroy(sphereButton);
            }
            else
            {
                Debug.LogWarning("指定されたオブジェクトが見つかりません: " + sphereButtonName);
            }
            SetBlueObject();
            LastTime = Time.time;


            StartTask();
            Count = 1;
            return;
        }

        if (!isTaskActive)
        {
            return; // タスクがアクティブでない場合、何もしない
        }
        // トリガーの入力をチェック


        // トリガーが押されていない場合にリセット
        if (rightTriggerValue == 0 && leftTriggerValue == 0)
        {
            ResetTriggerState();
        }

        // トリガーが押されている場合、次のオブジェクトの色を変更
        if (rightTriggerValue > triggerThreshold || leftTriggerValue > triggerThreshold)
        {
            // トリガーが押されていない場合にのみ次のオブジェクトを変更
            if (!isTriggerPressed)
            {
                ChangeColorToNextObject();
            }
            isTriggerPressed = true; // トリガーが押された状態にする
        }
    }


    // 次のオブジェクトの色を変更し、前回のオブジェクトの色をリセットする
    void ChangeColorToNextObject()
    {
        // 次のオブジェクトの色を変更
        if (j < sphereNames.Count) // jがリスト範囲内か確認
        {
            SelectTime = Time.time;


            TaskTime = SelectTime - LastTime;
            LastTime = SelectTime;

            CheckRayHit(currentHit, currentRayDirection, currentAnchorPosition);

            j++;
            ResetAllObjectsColor();
            SetBlueObject();
        }
        else
        {
            SceneManager.LoadScene("Start");
        }
    }

    // 最初の青いオブジェクトを設定する処理
    void SetBlueObject()
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
    }

    void SetCenterBlueObject()
    {
        GameObject targetObject = GameObject.Find("SphereButton");
        if (targetObject != null)
        {
            MeshRenderer renderer = targetObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.blue; // 最初のオブジェクトの色を青に変更
            }
            lastChangedObject = targetObject; // 最後に変更したオブジェクトを保存
        }
    }

    // トリガー状態をリセット
    void ResetTriggerState()
    {
        isTriggerPressed = false;
    }

    // すべてのオブジェクトの色をリセット
    void ResetAllObjectsColor()
    {
        // SphereNames1に含まれるオブジェクトの色を白にリセット
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
    public void CheckRayHit(RaycastHit hit, Vector3 rayDirection, Vector3 controllerPosition)
    {
        // 現在青いオブジェクト名を取得
        string currentBlueObjectName = sphereNames[j];



        // コントローラーから球への方向ベクトルを計算し、正規化
        Vector3 directionToSphere = spherePositions[currentBlueObjectName] - controllerPosition;
        Vector3 normalizedDirection = directionToSphere.normalized;


        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;
            // ヒットしたオブジェクトが青いオブジェクトと一致するかチェック
            if (hitObject.name == currentBlueObjectName)
            {
                successFlag = "1";
                // データをCSVファイルに書き込む処理を追加
                if (ModeManager.mode)
                {
                    WriteDataToCSV(successFlag, rayDirection, normalizedDirection, lastobject);
                }
                PlaySuccessSound();
            }
            else
            {
                successFlag = "0";
                // データをCSVファイルに書き込む処理を追加
                if (ModeManager.mode)
                {
                    WriteDataToCSV(successFlag, rayDirection, normalizedDirection, lastobject);
                }
                // 失敗音を再生
                PlayFailureSound();
            }
        }
        else
        {
            successFlag = "0";
            // データをCSVファイルに書き込む処理を追加
            if (ModeManager.mode)
            {
                WriteDataToCSV(successFlag, rayDirection, normalizedDirection, lastobject);
            }
            // 失敗音を再生
            PlayFailureSound();
        }

        lastobject = currentBlueObjectName;
    }

    void WriteDataToCSV(string successFlag, Vector3 rayDirection, Vector3 normalizedDirection, string lastsphere)
    {
        // 保存先ディレクトリ
        string directoryPath = Application.persistentDataPath;

        // フルパスを組み立て
        string filePath = Path.Combine(directoryPath, filename);

        // デバッグデータの収集
        List<string> lines = new List<string>();
        // ヘッダー行（ファイルが存在しない場合のみ追加）
        if (!File.Exists(filePath))
        {
            lines.Add("Scene,eSphereNam,SuccessFlag,SpherePositionX,SpherePositionY,SpherePositionZ,SphereDirectionX,SphereDirectionY,SphereDirectionZ,RayDirectionX,RayDirectionY,RayDirectionZ,Count,Tasktime," +
                      "rightControllerPositionX,rightControllerPositionY,rightControllerPositionZ," +
                      "rightControllerForwardX,rightControllerForwardY,rightControllerForwardZ," +
                      "leftControllerPositionX,leftControllerPositionY,leftControllerPositionZ," +
                      "leftControllerForwardX,leftControllerForwardY,leftControllerForwardZ");
        }



        // 現在のシーン名を取得
        Scene currentScene = SceneManager.GetActiveScene();

        // 必要なデータを取得
        string sphereName = sphereNames[j]; // 現在の球の名前
        string number = Regex.Match(sphereName, @"\d+").Value;
        Vector3 spherePosition = spherePositions[sphereName]; // 球の位置
        //Debug.Log(spherePosition);
        //Debug.Log(lastspherePosition);
        Vector3 leftControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        Vector3 rightControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

        Quaternion leftControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
        Quaternion rightControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        // コントローラの回転から方向ベクトルを取得（前方向）
        Vector3 leftControllerForward = leftControllerRotation * Vector3.forward;
        Vector3 rightControllerForward = rightControllerRotation * Vector3.forward;
        // 各データをCSV用にフォーマット
        string data = $"{currentScene.name},{sphereName},{successFlag}," +
                      $"{spherePosition.x},{spherePosition.y},{spherePosition.z}," +
                      $"{normalizedDirection.x},{normalizedDirection.y},{normalizedDirection.z}," +
                      $"{rayDirection.x},{rayDirection.y},{rayDirection.z}," +
                      $"{j},{TaskTime}," +
                      $"{rightControllerPosition.x},{rightControllerPosition.y},{rightControllerPosition.z}," +
                      $"{rightControllerForward.x},{rightControllerForward.y},{rightControllerForward.z}," +
                      $"{leftControllerPosition.x},{leftControllerPosition.y},{leftControllerPosition.z}," +
                      $"{leftControllerForward.x},{leftControllerForward.y},{leftControllerForward.z}";

        lines.Add(data);

        // CSVファイルにデータを書き込む（末尾に追記）
        File.AppendAllLines(filePath, lines);

        // CSVにデータを書き込む（末尾に追記）
    }

    void PlaySuccessSound()
    {
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound); // 成功音を再生
        }
    }
    void PlayFailureSound()
    {
        if (audioSource != null && failureSound != null)
        {
            audioSource.PlayOneShot(failureSound); // 失敗音を再生
        }
    }
}
