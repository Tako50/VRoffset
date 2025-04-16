using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;  // SceneManagement 名前空間を追加

public class HeadAndLeftControllerTracker : MonoBehaviour
{
    public OVRCameraRig cameraRig; // OVRCameraRig の参照
    private Transform centerEyeAnchor;
    [SerializeField]
    private string filename = "HeadAndLeftControllerTracker_05.csv"; // ファイル名を変更（拡張子を追加）
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
        Vector3 headDirection = centerEyeAnchor.forward; // ヘッドセットの前方方向
        Vector3 headEulerAngles = headRotation.eulerAngles; // オイラー角

        // 左コントローラの位置と向き
        Vector3 leftControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        Quaternion leftControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
        Vector3 leftControllerDirection = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) * Vector3.forward; // 左コントローラの前方方向
        Vector3 leftControllerEulerAngles = leftControllerRotation.eulerAngles; // 左コントローラのオイラー角

        // TargetManager から j を取得
        if (targetManager != null && targetManager.gameObject.activeInHierarchy)
        {
            count = targetManager.myintJ;  // targetManager がアクティブな場合
           // Debug.Log("TargetManager myintJ: " + count);
        }
        else if (targetManager2 != null && targetManager2.gameObject.activeInHierarchy)
        {
            count = targetManager2.myintJ;  // targetManager2 がアクティブな場合
            //Debug.Log("TargetManager2 myintJ: " + count);
        }
        else
        {
           //Debug.Log("Both TargetManager and TargetManager2 are not active.");
        }

        // データを書き込む
        WriteDataToCSV(headPosition, headRotation, headDirection, headEulerAngles, 
                       leftControllerPosition, leftControllerRotation, leftControllerDirection, 
                       leftControllerEulerAngles, count);
    }

    void WriteDataToCSV(Vector3 headPosition, Quaternion headRotation, Vector3 headDirection, Vector3 headEulerAngles, 
                        Vector3 leftControllerPosition, Quaternion leftControllerRotation, Vector3 leftControllerDirection, 
                        Vector3 leftControllerEulerAngles, int j)
    {
        // 保存先ディレクトリ
        string directoryPath = Application.persistentDataPath;
        string filePath = Path.Combine(directoryPath, filename);

        // ファイル内容を格納するリスト
        List<string> lines = new List<string>();

        // シーン名とカウントの取得
        string sceneName = SceneManager.GetActiveScene().name;
        int framecount = Time.frameCount;

        // ヘッダー行（ファイルが存在しない場合のみ追加）
        if (!File.Exists(filePath))
        {
            lines.Add("sceneName,frameCount,count," +
                      "headPosition.x,headPosition.y,headPosition.z," +
                      "headRotation.x,headRotation.y,headRotation.z," + // headRotation (クォータニオン)
                      "headDirection.x,headDirection.y,headDirection.z," + // headDirection (方向ベクトル)
                      "headEulerAngles.x,headEulerAngles.y,headEulerAngles.z," + // headEulerAngles (オイラー角)
                      "leftControllerPosition.x,leftControllerPosition.y,leftControllerPosition.z," +
                      "leftControllerRotation.x,leftControllerRotation.y,leftControllerRotation.z," +
                      "leftControllerDirection.x,leftControllerDirection.y,leftControllerDirection.z," +
                      "leftControllerEulerAngles.x,leftControllerEulerAngles.y,leftControllerEulerAngles.z");
        }

        // データ行
        string dataLine = $"{sceneName},{framecount},{j}," +
                           $"{headPosition.x},{headPosition.y},{headPosition.z}," +
                           $"{headRotation.x},{headRotation.y},{headRotation.z}," + // headRotation (クォータニオン)
                           $"{headDirection.x},{headDirection.y},{headDirection.z}," + // headDirection (方向ベクトル)
                           $"{headEulerAngles.x},{headEulerAngles.y},{headEulerAngles.z}," + // headEulerAngles (オイラー角)
                           $"{leftControllerPosition.x},{leftControllerPosition.y},{leftControllerPosition.z}," +
                           $"{leftControllerRotation.x},{leftControllerRotation.y},{leftControllerRotation.z}," +
                           $"{leftControllerDirection.x},{leftControllerDirection.y},{leftControllerDirection.z}," +
                           $"{leftControllerEulerAngles.x},{leftControllerEulerAngles.y},{leftControllerEulerAngles.z}";

        // データを追加
        lines.Add(dataLine);

        // ファイルを上書き保存（既存ファイルがあれば追記）
        File.AppendAllLines(filePath, lines);

        // Debug.Log("Data written to CSV successfully.");
    }
}
