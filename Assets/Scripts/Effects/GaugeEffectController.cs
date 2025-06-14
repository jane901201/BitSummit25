using UnityEngine;

public class GaugeEffectController : MonoBehaviour
{
    [Header("�G�t�F�N�g�̎Q�ƁiA, B = MAX�p / C, D = 0�p�j")]
    [SerializeField] private GameObject effectA;
    [SerializeField] private GameObject effectB;
    [SerializeField] private GameObject effectC;
    [SerializeField] private GameObject effectD;

    private GameManager gameManager;
    private bool Iskakusei;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        // �S�G�t�F�N�g���\����
        effectA.SetActive(false);
        effectB.SetActive(false);
        effectC.SetActive(false);
        effectD.SetActive(false);
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
        effectC.SetActive(c);
        effectD.SetActive(d);
    }

    public void KakuseiEffects()
    {
        SetEffects(false, false, true, true);
        Iskakusei = true;
    }
}
