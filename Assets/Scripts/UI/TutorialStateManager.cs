using UnityEngine;

public class TutorialStateManager : MonoBehaviour
{
    public static TutorialStateManager Instance;

    public bool HasShownTutorial { get; private set; } = false;

    private void Awake()
    {
        // シングルトンパターン
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // シーン遷移しても残す
    }

    public void MarkTutorialShown()
    {
        HasShownTutorial = true;
    }

    public void ResetTutorial()
    {
        HasShownTutorial = false;
    }
}
