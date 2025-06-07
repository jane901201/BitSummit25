using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ghosts;
using UnityEngine;
using UnityEngine.Serialization;
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
    
    [Header("Gauge")]
    [SerializeField] private int currentGauge = 0;
    [SerializeField] private int maxGauge = 100;
    [SerializeField] private float gaugeTime = 10f;

    [SerializeField] private GameObject PlayerPointer;
    [SerializeField] private float overlapThreshold = 20f;
    
    public static GameManager Instance { get; private set; }
    
    public int CurrentPlayerHp => currentPlayerHp;
    public int CurrentAttackPower => currentAttackPower;
    public int CurrentGauge => currentGauge;
    public int TotalScore => totalScore;
    
    
    private List<IGhost> ghostsList = new List<IGhost>();
    private List<IGhost> deadGhostsList = new List<IGhost>();
    private int currentDeadGhostCount = 0;

    
    private void Awake()
    {
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
        currentAttackPower = attackPower;
        currentPlayerHp = maxPlayerHp;
        StartCoroutine(SpawnGhost());
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
        if (currentGauge >= maxGauge)
        {
            
        }
        
        
    }

    private void AttackRange()
    {
        
    }

    #region UI

    public void HpBarUpdate()
    {
        
    }
    
    public void GaugeBarUpdate()
    {
        
    }

    public void ScoreUpdate()
    {
        
    }
    

    #endregion


    public void AddScore(int scoreValue)
    {
        totalScore += scoreValue;
        ScoreUpdate();
    }

    public void AddGauge(int gaugeValue)
    {
        currentGauge += gaugeValue;
        GaugeBarUpdate();
    }

    public void AddCurrentDeadGhostCount()
    {
        currentDeadGhostCount++;
    }
    
    public void TakeDamage(int damage)
    {
        currentPlayerHp -= damage;
        CheckGameResult();
    }

    public void TakeGhostsDamage(SwingDirection direction, SwingSpeed speed)
    {
        for (int i = ghostsList.Count - 1; i >= 0; i--)
        {
            if(ghostsList[i].GetIsAttackable(direction, speed))
            {
                ghostsList[i].TakeDamage(currentAttackPower);
            }
            if (ghostsList[i].IsDead())
            {
                ghostsList[i].Die();
                deadGhostsList.Add(ghostsList[i]);
                ghostsList.RemoveAt(i);
            }
        }
        deadGhostsList.ForEach(ghost => Destroy(ghost.gameObject));
        deadGhostsList.Clear();
        CheckGameResult();
    }

    public void RemoveGhost(IGhost ghost)
    {
        int index = deadGhostsList.FindIndex(x => x == ghost);
        if (index >= 0)
        {
            ghostsList[index].Die();
            deadGhostsList.Add(ghostsList[index]); 
        }
    }

    public void VisualOverlapDetector(GameObject object1, GameObject object2)
    {
        // Camera からスクリーン座標に変換
        Vector3 obj1ScreenPos = Camera.main.WorldToScreenPoint(object1.transform.position);
        Vector3 obj2ScreenPos = Camera.main.WorldToScreenPoint(object2.transform.position);

        // 距離を測定
        float screenDistance = Vector2.Distance(
            new Vector2(obj1ScreenPos.x, obj1ScreenPos.y),
            new Vector2(obj2ScreenPos.x, obj2ScreenPos.y)
        );

        // しきい値（ピクセル）で重なりを判定
        if (screenDistance < overlapThreshold)
        {
            // 見た目上重なっていると判定 → トリガー処理
            OnOverlapDetected();
        }

    }

    private void OnOverlapDetected()
    {
        Debug.Log("Overlap Detected!");   
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
