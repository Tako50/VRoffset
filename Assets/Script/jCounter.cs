using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class jCounter : MonoBehaviour
{
    [SerializeField]
    private TargetManager targetManager;
    [SerializeField]
    private TargetManager2 targetManager2; // targetManager2の参照を追加
    [SerializeField]
    private TextMeshProUGUI text;

    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        // targetManagerとtargetManager2が設定されているか確認する
        if (targetManager == null && targetManager2 == null)
        {
            Debug.LogError("Both TargetManager and TargetManager2 are missing.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // targetManager と targetManager2 のどちらがアクティブかを確認
        if (targetManager != null && targetManager.gameObject.activeInHierarchy)
        {
            count = targetManager.myintJ;  // targetManager がアクティブな場合
        }
        else if (targetManager2 != null && targetManager2.gameObject.activeInHierarchy)
        {
            count = targetManager2.myintJ;  // targetManager2 がアクティブな場合
        }
        else
        {
            Debug.Log("Both TargetManager and TargetManager2 are not active.");
        }

        // テキストを更新
        text.text = "count: " + count;
    }


}
