using UnityEngine;

public class PointerEffectController : MonoBehaviour
{
    public ParticleSystem[] particleSystems;

    private Vector3 lastPosition;
    private float speed;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // 移動速度を計算
        speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;

        // 全てのパーティクルシステムに適用
        foreach (var ps in particleSystems)
        {
            var emission = ps.emission;
            emission.rateOverTime = speed*10;
        }
    }
}
