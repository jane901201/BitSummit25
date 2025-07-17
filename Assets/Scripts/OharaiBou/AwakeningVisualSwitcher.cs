using UnityEngine;

public class AwakeningVisualSwitcher : MonoBehaviour
{
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material enhancedMaterial;
    private Renderer objectRenderer;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogWarning($"{gameObject.name} Ç… Renderer Ç™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÇ≈ÇµÇΩÅB");
        }
    }

    public void SetAwakeningState(bool isEnhanced)
    {
        if (objectRenderer != null)
        {
            objectRenderer.material = isEnhanced ? enhancedMaterial : normalMaterial;
        }
    }
}
