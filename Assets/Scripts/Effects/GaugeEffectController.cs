using UnityEngine;

public class GaugeEffectController : MonoBehaviour
{
    [Header("エフェクトの参照（A, B = MAX用 / C, D = 0用）")]
    [SerializeField] private GameObject effectA;
    [SerializeField] private GameObject effectB;

    private GameManager gameManager;
    private bool Iskakusei;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        // 全エフェクトを非表示に
        effectA.SetActive(false);
        effectB.SetActive(false);
    }

    void Update()
    {
        int current = gameManager.GetCurrentGauge();
        int max = gameManager.GetMaxGauge();

        //Debug.Log(current);

        if (current >= max)
        {
            SetEffects(true, true, false, false);
        }
        else
        {
            SetEffects(false, false, false, false);
        }
    }

    private void SetEffects(bool a, bool b, bool c, bool d)
    {
        effectA.SetActive(a);
        effectB.SetActive(b);
    }

    public void KakuseiEffects()
    {
        SetEffects(false, false, true, true);
        Iskakusei = true;
    }
}
