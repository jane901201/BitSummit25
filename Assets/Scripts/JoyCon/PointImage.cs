using UnityEngine;
using UnityEngine.Serialization;

public class PointerImage : MonoBehaviour
{
    [SerializeField] private Transform sphereTransfrom;
    private RectTransform rectTransform;
    // 任意の倍率、大きくすると小さい動きで大きく動く
    private float scale = 50.0f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        float x = sphereTransfrom.position.x * scale;
        float y = sphereTransfrom.position.z * -scale;
        rectTransform.anchoredPosition = new Vector3(x, y, 0);
    }
}