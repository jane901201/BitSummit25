using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject targetUI; // 表示・非表示を切り替えるUI

    void Start()
    {
        // ゲーム開始時に非表示にする
        if (targetUI != null)
        {
            targetUI.SetActive(false);
        }
    }

    // 他スクリプトから呼び出す関数
    public void ShowUI()
    {
        if (targetUI != null)
        {
            targetUI.SetActive(true);
        }
    }
}
