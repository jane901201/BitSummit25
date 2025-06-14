using System.Collections;
using System.Collections.Generic;
using Ghosts;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> ghosts;

    [SerializeField] private int totalScore = 0;
    [SerializeField] private int maxPlayerHp = 100; 
    [SerializeField] private int currentPlayerHp = 0;
    [SerializeField] private Transform attackOriginalRangePoint;
    
    [Header("Ghost")]
    [SerializeField] private float ghostSpawnTime = 4f;
    [SerializeField] private Transform ghostSpawnPoint;
    [SerializeField] private Vector3 spawnRange = new Vector3(5f, 0f, 5f);
    [SerializeField] private int maxGhostCount = 20;
    
    [Header("Attack")]
    [SerializeField] private int attackPower = 10;
    [SerializeField] private int enhancedAttackPower = 20;
    [SerializeField] private int currentAttackPower = 0;
    [SerializeField] private GameObject defaultAttackableTrigger;
    [SerializeField] private GameObject enhancedAttackableTrigger;
    
    [Header("Gauge")]
    [SerializeField] private int currentGauge = 0;
    [SerializeField] private int maxGauge = 100;
    [SerializeField] private float gaugeTime = 10f;

    [SerializeField] private GameObject playerPointer;
    [SerializeField] private float overlapThreshold = 20f;
    
    [Header("UI")]
    [SerializeField] UIManager uiManager;
    
    public static GameManager Instance { get; private set; }
    
    public int CurrentPlayerHp => currentPlayerHp;
    public int CurrentAttackPower => currentAttackPower;
    public int CurrentGauge => currentGauge;
    public int TotalScore => totalScore;
    
    [SerializeField]
    private List<IGhost> ghostsList = new List<IGhost>();
    
    private List<IGhost> deadGhostsList = new List<IGhost>();
    private int currentDeadGhostCount = 0;
    private bool isEnhanced = false;
    
    private void Awake()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    } 

    private void Start()
    {
        uiManager.InitinalHpPanel(maxPlayerHp);
        currentAttackPower = attackPower;
        currentPlayerHp = maxPlayerHp;
        StartCoroutine(SpawnGhost());
        defaultAttackableTrigger.SetActive(true);
        enhancedAttackableTrigger.SetActive(false);
    }

    //TODO:生成範囲はどこですか？
    private IEnumerator SpawnGhost()
    {
        yield return new WaitForSeconds(ghostSpawnTime);
        int index = Random.Range(0, ghosts.Count);
        
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRange.x, spawnRange.x),
            Random.Range(-spawnRange.y, spawnRange.y),
            Random.Range(-spawnRange.z, spawnRange.z)
        );

        // 基準点からオフセットを加えた位置に生成
        Vector3 spawnPosition = ghostSpawnPoint.position + randomOffset;
        
        GameObject ghost = Instantiate(ghosts[index], spawnPosition, ghostSpawnPoint.rotation);
        ghostsList.Add(ghost.GetComponent<IGhost>());
        StartCoroutine(SpawnGhost());
    }

    private void Update()
    {
        if (!isEnhanced && currentGauge >= maxGauge)
        {
            StartCoroutine(GaugeCoroutine());
        }
        
        CheckVisualOverlaps();
    }

    private IEnumerator GaugeCoroutine()
    {
        isEnhanced = true;

        // 強化状態に切り替え
        currentAttackPower = enhancedAttackPower;
        defaultAttackableTrigger.SetActive(false);
        enhancedAttackableTrigger.SetActive(true);

        Debug.Log("強化モード突入！");

        yield return new WaitForSeconds(gaugeTime);

        // 元に戻す
        currentAttackPower = attackPower;
        defaultAttackableTrigger.SetActive(true);
        enhancedAttackableTrigger.SetActive(false);
        currentGauge = 0;
        isEnhanced = false;

        Debug.Log("強化モード終了");
    }

    #region UI
    
    public void ScoreUpdate()
    {
        
    }


    #endregion

    public int GetCurrentGauge() => currentGauge;
    public int GetMaxGauge() => maxGauge;

    public void AddScore(int scoreValue)
    {
        totalScore += scoreValue;
        ScoreUpdate();
    }

    public void AddGauge(int gaugeValue)
    {
        currentGauge += gaugeValue;
    }

    public void AddCurrentDeadGhostCount()
    {
        currentDeadGhostCount++;
    }
    
    public void TakeDamage(int damage)
    {
        currentPlayerHp -= damage;
        uiManager.SetHpPanel(currentPlayerHp);
        CheckGameResult();
    }

    public void TakeGhostsDamage(SwingDirection direction, SwingSpeed speed)
    {
        for (int i = ghostsList.Count - 1; i >= 0; i--)
        {
            if(ghostsList[i].GetIsAttackable(direction, speed))
            {
                SoundManager.Instance.PlayDamageMakeSound(); // 追加
                ghostsList[i].TakeDamage(currentAttackPower);
            }
            if (ghostsList[i].IsDead())
            {
                ghostsList[i].Die();
                deadGhostsList.Add(ghostsList[i]);
            }
        }
        ghostsList.RemoveAll(ghost => ghost.IsDead());
        deadGhostsList.ForEach(ghost => Destroy(ghost.gameObject));
        deadGhostsList.Clear();
        CheckGameResult();
    }
    
    public void CheckVisualOverlaps()
    {
        //Debug.Log("CheckVisualOverlaps");
        Vector3 pointerScreenPos = Camera.main.WorldToScreenPoint(playerPointer.transform.position);

        foreach (var ghost in ghostsList)
        {
            if (ghost == null) continue;

            Vector3 ghostScreenPos = Camera.main.WorldToScreenPoint(ghost.gameObject.transform.position);

            float screenDistance = Vector2.Distance(
                new Vector2(pointerScreenPos.x, pointerScreenPos.y),
                new Vector2(ghostScreenPos.x, ghostScreenPos.y)
            );

            if (screenDistance < overlapThreshold)
            {
                OnOverlapDetected(ghost); // どのゴーストが当たったか渡せるように
            }
        }
    }
    
    //プレイヤーのポインターが鬼から外れた
    public void ResetOverlapDetectedFlag()
    {
        Debug.Log("ResetOverlapDetectedFlag");
        foreach (var ghost in ghostsList)
        {
            ghost.IsOverlapDetected = false;
        }
    }
    
    //プレイヤーのポインターが鬼に当たっている
    private void OnOverlapDetected(IGhost ghost)
    {
        Debug.Log("Overlap Detected!");
        ghost.IsOverlapDetected = true;
    }

    public void CheckGameResult()
    {
        if (currentPlayerHp <= 0)
        {
            GameOver();
        }
        else if(currentDeadGhostCount == maxGhostCount)
        {
            Victory();
        }
    }

    public void Victory()
    {
        Debug.Log("Victory");
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
    }
}
