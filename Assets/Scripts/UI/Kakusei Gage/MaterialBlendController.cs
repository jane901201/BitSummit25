using UnityEngine;

public class MaterialBlendController : MonoBehaviour
{
    [SerializeField] private Material targetMaterial;

    private float objectHeight;

    void Start()
    {
        // �I�u�W�F�N�g�̍����������v�Z
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            objectHeight = rend.bounds.size.y;
            targetMaterial.SetFloat("_ObjectHeight", objectHeight);
        }
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        float current = GameManager.Instance.GetCurrentGauge()*40/150;
        float max = GameManager.Instance.GetMaxGauge();

        if (max <= 0f) return; // 0���Z�h�~

        float blendValue = Mathf.Clamp01(current / max);
        targetMaterial.SetFloat("_BlendAmount", blendValue);
    }
}

