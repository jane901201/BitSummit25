using UnityEngine;
using Controller; // DrawTracker の名前空間

public class PointerEffectSwitcher : MonoBehaviour
{
    [Header("エフェクト")]
    public GameObject trailNormal;
    public GameObject trailFast;
    public GameObject trailPowerUp;
    public GameObject particleNormal;
    public GameObject particleFast;
    public GameObject particlePowerUp1;
    public GameObject particlePowerUp2;
    public GameObject particlePowerUp3;
    public GameObject particlePowerUp4;

    [Header("参照するスクリプト")]
    public GameManager gameManager;        // Inspectorでアサイン

    [Header("切り替えしきい値")]
    public float fastSpeedThreshold = 5f;

    private Vector3 lastPosition;
    private float speed;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {

        if (gameManager == null)
        {
            //Debug.LogError("gameManager が null です！");
            return;
        }

        bool isPowerUpMode = gameManager.IsEnhanced;

        // 移動速度を計算
        speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;

        if (isPowerUpMode)
        {
            Debug.Log(isPowerUpMode);
            // 覚醒モード
            trailNormal.SetActive(false);
            trailFast.SetActive(false);
            trailPowerUp.SetActive(true);

            particleNormal.SetActive(false);
            particleFast.SetActive(false);

            particlePowerUp1.SetActive(true);
            particlePowerUp2.SetActive(true);
            particlePowerUp3.SetActive(true);
            particlePowerUp4.SetActive(true);
        }
        else
        {
            // 覚醒じゃないとき
            trailPowerUp.SetActive(false);
            particlePowerUp1.SetActive(false);
            particlePowerUp2.SetActive(false);
            particlePowerUp3.SetActive(false);
            particlePowerUp4.SetActive(false);

            if (speed > fastSpeedThreshold)
            {
                trailNormal.SetActive(false);
                particleNormal.SetActive(false);
                trailFast.SetActive(true);
                particleFast.SetActive(true);
            }
            else
            {
                trailNormal.SetActive(true);
                particleNormal.SetActive(true);
                trailFast.SetActive(false);
                particleFast.SetActive(false);
            }
        }
    }
}

