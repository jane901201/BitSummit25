using UnityEngine;

public class RankModel : MonoBehaviour
{
    [SerializeField] private Sprite rankD, rankC, rankB, rankA, rankS;
    [SerializeField] private SpriteRenderer rankRenderer; // SpriteRenderer‚ğİ’è

    private void Start()
    {
        int score = PhantomSwing.Instance.GameData.GetScore();

        if (score <= 10)
        {
            rankRenderer.sprite = rankD;
        }
        else if (score >= 40)
        {
            rankRenderer.sprite = rankA;
        }
        else if (score >= 30)
        {
            rankRenderer.sprite = rankB;
        }
        else if (score >= 20)
        {
            rankRenderer.sprite = rankC;
        }
        else
        {
            rankRenderer.sprite = rankS;
        }
    }
}
