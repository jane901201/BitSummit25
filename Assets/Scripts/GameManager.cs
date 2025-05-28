using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private int currentGhostCount = 0;
    
    [Header("Attack")]
    [SerializeField] private int attackPower = 10;
    [SerializeField] private int enhancedAttackPower = 20;
    [SerializeField] private int currentAttackPower = 0;
    
    [Header("Gauge")]
    [SerializeField] private int currentGauge = 0;
    [SerializeField] private int maxGauge = 100;
    [SerializeField] private float gaugeTime = 10f;
    
    public static GameManager Instance { get; private set; }
    
    public int CurrentPlayerHp => currentPlayerHp;
    public int CurrentAttackPower => currentAttackPower;
    public int CurrentGauge => currentGauge;
    public int TotalScore => totalScore;
    public int CurrentGhostCount => currentGhostCount;
    
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
        
        Instantiate(ghosts[index], spawnPosition, ghostSpawnPoint.rotation);
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
    
    public void TakeDamage(int damage)
    {
        currentPlayerHp -= damage;
    }

    public void CheckGameResult()
    {
        if (currentPlayerHp <= 0)
        {
            GameOver();
        }
        else if(currentGhostCount == maxGhostCount)
        {
            Victory();
        }
    }

    public void Victory()
    {
        
    }

    public void GameOver()
    {
        
    }
}
