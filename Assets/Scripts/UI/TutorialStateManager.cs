using UnityEngine;

public class TutorialStateManager : MonoBehaviour
{
    public static TutorialStateManager Instance;

    public bool HasShownTutorial { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void MarkTutorialShown()
    {
        HasShownTutorial = true;
        Debug.Log("[Tutorial] Marked as shown.");
    }

    public void ResetTutorial()
    {
        HasShownTutorial = false;
        Debug.Log("[Tutorial] Reset tutorial.");
    }
}
