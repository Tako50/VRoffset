using UnityEngine;
using System.Text;

public class SceneHierarchyExporter : MonoBehaviour
{
    void Start()
    {
        StringBuilder hierarchyLog = new StringBuilder();
        hierarchyLog.AppendLine("Scene hierarchy:");

        // シーン全体のオブジェクトを取得
        foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            AppendHierarchy(obj.transform, 0, hierarchyLog);
        }

        // 階層全体を1つのログとして出力
        Debug.Log(hierarchyLog.ToString());
    }

    void AppendHierarchy(Transform parent, int depth, StringBuilder log)
    {
        // インデント（深さに応じてスペースを追加）
        string indent = new string(' ', depth * 2);
        log.AppendLine(indent + parent.name);

        // 子オブジェクトの処理
        foreach (Transform child in parent)
        {
            AppendHierarchy(child, depth + 1, log);
        }
    }
}
