using UnityEngine;
using UnityEngine.UI;

public class RankUI : MonoBehaviour
{
    [SerializeField] private Sprite rankC, rankB, rankA, rankS;
    [SerializeField] private Image rankImage;
    
    
    
    private void Start()
    {
        int score = PhantomSwing.Instance.GameData.GetScore();
        
        if (score <= 10)
        {
            rankImage.sprite = rankC;
        }
        else if (score >= 20)
        {
            rankImage.sprite = rankB;
        }
        else if (score >= 30)
        {
            rankImage.sprite = rankA;
        }
        else
        {
            rankImage.sprite = rankS;
        }
    }
}
