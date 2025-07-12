using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhantomSwing: MonoBehaviour
{
    [SerializeField] private float overlapThreshold = 0.1f;
    [SerializeField] private GameObject playerPointer;
    [SerializeField] private GameData gameData;

    public GameData GameData
    {
        get { return gameData; }
        set { gameData = value; }
    }

    public static PhantomSwing Instance { get; private set; }

    public GameObject PlayerPointer
    {
        get { return playerPointer; }
        set { playerPointer = value; }
    }
    
    
    private void Awake()
    { 
        Instance = this;
        gameData.SetScore(0);
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        
    }

    public void LoadGameScene(String sceneName)
    {
        playerPointer = null;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadGameClearScene()
    {
        
    }

    public void LoadGameOverScene()
    {
        
    }

    public void LoadTitleScene()
    {
        
    }
    
    public bool CheckVisualOverlaps_Viewport(GameObject gameObject)
    {
        // Pointer の Viewport 座標（x: 横方向の割合, y: 縦方向の割合, z: カメラからの距離）
        Vector3 pointerViewport = Camera.main.WorldToViewportPoint(playerPointer.transform.position);
        
        Vector3 gameObjectViewport = Camera.main.WorldToViewportPoint(gameObject.transform.position);
        
        // Viewport 座標上での2D距離（XYだけ使う）
        float dist = Vector2.Distance(
            new Vector2(pointerViewport.x, pointerViewport.y),
            new Vector2(gameObjectViewport.x, gameObjectViewport.y)
        );

        // しきい値は Viewport 単位なので 0.05 ～ 0.1 程度が目安
        if (dist < overlapThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
