using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    // タスクの状態を保持するフラグ
    private bool isTaskRunning = false;

    // UIボタンの参照
    public Button startButton;

    void Start()
    {
        // ボタンにクリック時の動作を追加
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartTask);
        }
    }

    void StartTask()
    {
        if (!isTaskRunning) // タスクが開始されていない場合のみ実行
        {
            isTaskRunning = true;
            Debug.Log("Task has started!");
            // ここにタスクを開始する処理を記述
        }
        else
        {
            Debug.Log("Task is already running.");
        }
    }
}
