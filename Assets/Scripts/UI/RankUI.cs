using UnityEngine;

public class RankModel : MonoBehaviour
{
    [SerializeField] private Sprite rankD, rankC, rankB, rankA, rankS;
    [SerializeField] private SpriteRenderer rankRenderer; // SpriteRenderer‚ğİ’è

    private void Start()
    {
        int score = PhantomSwing.Instance.GameData.GetScore();

        if (score < 700)
        {
            rankRenderer.sprite = rankD;
        }
        else if (score < 800)
        {
            rankRenderer.sprite = rankC;
        }
        else if (score < 900)
        {
            rankRenderer.sprite = rankB;
        }
        else if (score < 1000)
        {
            rankRenderer.sprite = rankA;
        }
        else
        {
            rankRenderer.sprite = rankS;
        }
    }
}
