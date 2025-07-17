using TMPro;
using UnityEngine;

public class KillCountDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro killCountText;

    private void Start()
    {
        // スコア取得（敵を倒した数）
        int score = PhantomSwing.Instance.GameData.GetScore();

        // テキストを更新
        killCountText.text = $"倒した敵の数：{score}";
    }
}
