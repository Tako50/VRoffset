using UnityEngine;

public class GameManager : MonoBehaviour
{
    // TargetManagerのインスタンスを取得
    public TargetManager targetManager;

    // ゲーム開始のフラグ
    private bool gameStarted = false;

    void Start()
    {
        // ゲームが開始される前にTargetManagerのStartTaskを呼び出す
        if (targetManager != null)
        {
            // 初期状態でターゲットマネージャーのタスクを開始しないようにする
            targetManager.StartTask(); // 初期化はしないが、必要ならここで呼び出す
        }
    }

    // ゲーム開始メソッド
    public void StartGame()
    {
        if (!gameStarted) // ゲームがまだ開始されていなければ
        {
            gameStarted = true; // ゲーム開始フラグを設定
            Debug.Log("Game Started!");
            

            // ターゲットマネージャーのタスクを開始
            if (targetManager != null)
            {
                targetManager.StartTask(); // ゲーム開始時にタスクを開始
            }
        }
    }
}
