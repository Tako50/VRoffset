using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement; // SceneManagement 名前空間を追加

public class HeadAndControllerTracker : MonoBehaviour
{
    public OVRCameraRig cameraRig; // OVRCameraRig の参照
    private Transform centerEyeAnchor;

    [SerializeField]
    private string filename = "HeadAndControllerTracker_21.csv"; // ファイル名を変更（拡張子を追加）
    [SerializeField]
    private TargetManager targetManager;
    [SerializeField]
    private TargetManager2 targetManager2; // targetManager2の参照を追加
    private int count = 0;

    void Start()
    {
        if (cameraRig == null)
        {
            cameraRig = FindObjectOfType<OVRCameraRig>(); // シーン内の OVRCameraRig を取得
        }

        if (cameraRig != null)
        {
            centerEyeAnchor = cameraRig.centerEyeAnchor; // centerEyeAnchor を取得
        }
        else
        {
            Debug.LogError("OVRCameraRig が見つかりません。シーンに配置してください。");
        }
    }

    void Update()
    {
        if (centerEyeAnchor == null)
        {
            Debug.LogWarning("centerEyeAnchor が見つかりません。");
            return;
        }

        // ヘッドセット（頭）の位置と向き
        Vector3 headPosition = centerEyeAnchor.position;
        Quaternion headRotation = centerEyeAnchor.rotation;

        // 右コントローラの位置と向き
        Vector3 rightControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion rightControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        // 左コントローラの位置と向き
        Vector3 leftControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        Quaternion leftControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);

        // TargetManager から j を取得
        if (targetManager != null && targetManager.gameObject.activeInHierarchy)
        {
            count = targetManager.myintJ; // targetManager がアクティブな場合
        }
        else if (targetManager2 != null && targetManager2.gameObject.activeInHierarchy)
        {
            count = targetManager2.myintJ; // targetManager2 がアクティブな場合
        }

        // データを書き込む
        WriteDataToCSV(headPosition, headRotation, rightControllerPosition, rightControllerRotation, leftControllerPosition, leftControllerRotation, count);
    }

    void WriteDataToCSV(Vector3 headPosition, Quaternion headRotation, Vector3 rightControllerPosition, Quaternion rightControllerRotation, Vector3 leftControllerPosition, Quaternion leftControllerRotation, int j)
    {
        // 保存先ディレクトリ
        string directoryPath = Application.persistentDataPath;
        string filePath = Path.Combine(directoryPath, filename);

        // ファイル内容を格納するリスト
        List<string> lines = new List<string>();

        // シーン名とカウントの取得
        string sceneName = SceneManager.GetActiveScene().name; // 現在のシーン名を取得
        int framecount = Time.frameCount; // フレーム数をカウント

        // ヘッダー行（ファイルが存在しない場合のみ追加）
        if (!File.Exists(filePath))
        {
            lines.Add("sceneName,frameCount,count," +
                      "headPosition.x,headPosition.y,headPosition.z,headRotation.x,headRotation.y,headRotation.z," +
                      "rightControllerPosition.x,rightControllerPosition.y,rightControllerPosition.z,rightControllerRotation.x,rightControllerRotation.y,rightControllerRotation.z," +
                      "leftControllerPosition.x,leftControllerPosition.y,leftControllerPosition.z,leftControllerRotation.x,leftControllerRotation.y,leftControllerRotation.z");
        }

        // データ行
        string dataLine = $"{sceneName},{framecount},{count}," +
                           $"{headPosition.x},{headPosition.y},{headPosition.z}," +
                           $"{headRotation.x},{headRotation.y},{headRotation.z}," +
                           $"{rightControllerPosition.x},{rightControllerPosition.y},{rightControllerPosition.z}," +
                           $"{rightControllerRotation.x},{rightControllerRotation.y},{rightControllerRotation.z}," +
                           $"{leftControllerPosition.x},{leftControllerPosition.y},{leftControllerPosition.z}," +
                           $"{leftControllerRotation.x},{leftControllerRotation.y},{leftControllerRotation.z}";

        // データを追加
        lines.Add(dataLine);

        // ファイルを上書き保存（既存ファイルがあれば追記）
        File.AppendAllLines(filePath, lines);

        // Debug.Log("Data written to CSV successfully.");
    }
}
