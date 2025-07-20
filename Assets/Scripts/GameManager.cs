using System.Collections;
using System.Collections.Generic;
using Controller;
using Controller.PC;
using Ghosts;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> ghosts;

    [SerializeField] private int totalScore = 0;
    [SerializeField] private int maxPlayerHp = 100; 
    [SerializeField] private int currentPlayerHp = 0;
    [SerializeField] private Transform attackOriginalRangePoint;

    [Header("Ghost")]
    [SerializeField] public GameObject ghostGroup;

    [SerializeField] private float ghostSpawnTime = 4f;
    [SerializeField] private Transform ghostSpawnPoint;
    [SerializeField] private Vector3 spawnRange = new Vector3(5f, 0f, 5f);
    [SerializeField] private int maxGhostCount = 20;
    [SerializeField] private float isAttackableDistanceBetweenGhosts = 1f;
    
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

    private PositionClamper positionClamper;



    [SerializeField]
    private List<IGhost> ghostsList = new List<IGhost>();
    
    private List<IGhost> deadGhostsList = new List<IGhost>();
    private int currentDeadGhostCount = 0;
    private bool isEnhanced = false;
   
    public bool IsEnhanced => isEnhanced;

    [Header("Material")]
    [SerializeField] private List<AwakeningVisualSwitcher> awakeningObjects; // Inspectorで対象を設定

    [System.Serializable]
    public class GhostSpawnInfo
    {
        public GameObject ghostPrefab;
        [Range(0f, 1f)]
        public float[] spawnRatesPerWave; // Waveごとのスポーン率（例：Wave1〜5で5要素）
    }

    [SerializeField]
    private List<GhostSpawnInfo> ghostSpawnInfos;

    private int currentWave = 0; // Wave数をGameManagerにも保持

    [System.Serializable]
    public class WaveSpawnSettings
    {
        public int waveNumber;
        public float maxSpawnTime;  // このWaveの最大SpawnTime
        public float minSpawnTime;  // ゴーストが0体のときの最小SpawnTime
    }

    [Header("Spawn Timing Settings")]
    [SerializeField]
    private List<WaveSpawnSettings> waveSpawnSettingsList;

    [Header("Game Clear Settings")]
    private float clearCountdownDuration = 30f;
    private float countdownWarningStart = 5f;
    private bool isClearCountdownStarted = false;


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
        
        ghostGroup = GameObject.Find("Ghosts").gameObject;
        positionClamper = new PositionClamper(Camera.main);
        //playerPointer.transform.localPosition = new Vector3(0f, 0f, -3.15785027f);
    } 

    private void Start()
    {
        uiManager.InitinalHpPanel(maxPlayerHp);
        currentAttackPower = attackPower;
        currentPlayerHp = maxPlayerHp;
        defaultAttackableTrigger.SetActive(true);
        enhancedAttackableTrigger.SetActive(false);
        PhantomSwing.Instance.PlayerPointer = playerPointer;
        JoystickController joystickController = playerPointer.GetComponent<JoystickController>();
        PCController pcController = playerPointer.GetComponent<PCController>();
        AccelerometerReader accelerometerReader = playerPointer.GetComponent<AccelerometerReader>();
        MoveWithAcceleration moveWithAcceleration = playerPointer.GetComponent<MoveWithAcceleration>();
        JoyconManager joyconManager = playerPointer.GetComponent<JoyconManager>();
        JoyconCursorMover joyconCursorMover = playerPointer.GetComponent<JoyconCursorMover>();
        PhantomSwing.Instance.DeviceSetting(joystickController, pcController, accelerometerReader, moveWithAcceleration, joyconManager, joyconCursorMover);
        StartCoroutine(SpawnGhost());
        ResetPlayerPosition();
    }

    public void ResetPlayerPosition()
    {
        //// 視窗内に制限された座標を取得
        //Vector3 clampedPosition = PositionClamper.ClampToViewport(new Vector3(0f, 0f, -3.15785027f), Camera.main);

        //// Z座標を維持して位置を更新
        //transform.position = new Vector3(clampedPosition.x, clampedPosition.y, transform.position.z);

        //playerPointer.transform.localPosition = new Vector3(0f, 0f, -3.15785027f);

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Vector3 offset = positionClamper.MoveTransformInsideScreen(mouseScreenPos, playerPointer.transform) - playerPointer.transform.position;
        //offset.z = -3.15785f;
        Debug.Log(offset);
        playerPointer.transform.localPosition += offset;
    }

    private void LateUpdate()
    {
        StartCoroutine(FixTransformZ());
    }

    private IEnumerator FixTransformZ()
    {
        yield return new WaitForSeconds(3f);
        if (playerPointer.transform.localPosition.z != -3.15785f)
        {
            Vector3 pos = playerPointer.transform.localPosition;
            pos.z = -3.15785f;
            playerPointer.transform.localPosition = pos;
        }
    }

    private IEnumerator SpawnGhost()
    {
        float spawnTime = GetDynamicSpawnTime();
        yield return new WaitForSeconds(spawnTime);

        if (ghostsList.Count >= maxGhostCount)
        {
            StartCoroutine(SpawnGhost());
            yield break;
        }

        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRange.x, spawnRange.x),
            Random.Range(-spawnRange.y, spawnRange.y),
            Random.Range(-spawnRange.z, spawnRange.z)
        );

        Vector3 spawnPosition = ghostSpawnPoint.position + randomOffset;
        GameObject selectedGhost = GetRandomGhostForCurrentWave();

        GameObject ghost = Instantiate(selectedGhost, spawnPosition, ghostSpawnPoint.rotation);
        ghost.transform.SetParent(ghostGroup.transform, true);
        ghost.transform.parent = null;

        ghostsList.Add(ghost.GetComponent<IGhost>());
        StartCoroutine(SpawnGhost());
    }


    private void Update()
    {
        // ゲージが最大、かつまだ覚醒していない、かつ左クリックされた場合に覚醒
        if (!isEnhanced && currentGauge >= maxGauge && Input.GetMouseButtonDown(0))
        {
            
            StartCoroutine(GaugeCoroutine());
        }

        CheckVisualOverlaps_Viewport();

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPlayerPosition();
        }
    }

    public void Gauge()
    {
        if (!isEnhanced && currentGauge >= maxGauge)
        {
            
            StartCoroutine(GaugeCoroutine());
        }
    }


    public IEnumerator GaugeCoroutine()
    {
        isEnhanced = true;
        currentAttackPower = enhancedAttackPower;
        defaultAttackableTrigger.SetActive(false);
        enhancedAttackableTrigger.SetActive(true);

        // 覚醒ビジュアルON
        foreach (var obj in awakeningObjects)
        {
            obj.SetAwakeningState(true);
        }

        Debug.Log("強化モード突入！");

        yield return new WaitForSeconds(gaugeTime);

        currentAttackPower = attackPower;
        defaultAttackableTrigger.SetActive(true);
        enhancedAttackableTrigger.SetActive(false);
        currentGauge = 0;
        isEnhanced = false;

        // 覚醒ビジュアルOFF
        foreach (var obj in awakeningObjects)
        {
            obj.SetAwakeningState(false);
        }

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
            if (isEnhanced)
            {
                if(ghostsList[i].GetIsAttackableRange())
                {
                    SoundManager.Instance.PlayDamageMakeSound(); // 追加
                    ghostsList[i].TakeDamage(currentAttackPower);
                }
            }
            else
            {
                if(ghostsList[i].GetIsAttackable(direction, speed))
                {
                    bool isAttackable = true;
                    for (int j = 0; j < ghostsList.Count; j++)
                    {
                        
                        //TODO:2つのオブジェクトのX距離が isAttackableDistanceBetweenGhosts より小さく、
                        //かつこの i オブジェクトが j オブジェクトの後ろにいる。
                        
                        Vector3 ghostI = Camera.main.WorldToViewportPoint(ghostsList[i].gameObject.transform.position);
                        Vector3 ghostJ = Camera.main.WorldToViewportPoint(ghostsList[j].gameObject.transform.position);
                        
                        float dist = Vector2.Distance(
                            new Vector2(ghostI.x, ghostI.y),
                            new Vector2(ghostJ.x, ghostJ.y)
                        );
                        
                        Debug.Log("distance " + dist + "isAttackableDistanceBetweenGhosts " +
                                  isAttackableDistanceBetweenGhosts + 
                                 "Attaced ghost " + ghostsList[i].gameObject.transform.position.z + 
                                  " Formor ghost " + ghostsList[j].gameObject.transform.position.z);
                        
                        if (dist <= isAttackableDistanceBetweenGhosts
                            && ghostsList[i].gameObject.transform.position.z
                            > ghostsList[j].gameObject.transform.position.z)
                        {
                            isAttackable = false;
                        }
                    }

                    if (isAttackable)
                    {
                        SoundManager.Instance.PlayDamageMakeSound(); // 追加
                        ghostsList[i].TakeDamage(currentAttackPower);
                    }
                }
            }
            if (ghostsList[i].IsDead())
            {
                ghostsList[i].Die();
                deadGhostsList.Add(ghostsList[i]);

                AddCurrentDeadGhostCount(); // ゴーストの撃破数をカウント
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
    
    public void CheckVisualOverlaps_Viewport()
    {
        // Pointer の Viewport 座標（x: 横方向の割合, y: 縦方向の割合, z: カメラからの距離）
        Vector3 pointerViewport = Camera.main.WorldToViewportPoint(playerPointer.transform.position);

        foreach (var ghost in ghostsList)
        {
            if (ghost == null) continue;

            Vector3 ghostViewport = Camera.main.WorldToViewportPoint(ghost.transform.position);

            // zが負ならカメラの背後にいるのでスキップ
            if (ghostViewport.z < 0 || pointerViewport.z < 0) continue;

            // Viewport 座標上での2D距離（XYだけ使う）
            float dist = Vector2.Distance(
                new Vector2(pointerViewport.x, pointerViewport.y),
                new Vector2(ghostViewport.x, ghostViewport.y)
            );

            // しきい値は Viewport 単位なので 0.05 ～ 0.1 程度が目安
            if (dist < overlapThreshold)
            {
                OnOverlapDetected(ghost);
            }
        }
    }
    
    //プレイヤーのポインターが鬼から外れた
    public void ResetOverlapDetectedFlag()
    {
        //Debug.Log("ResetOverlapDetectedFlag");
        foreach (var ghost in ghostsList)
        {
            ghost.IsOverlapDetected = false;
        }
    }
    
    //プレイヤーのポインターが鬼に当たっている
    private void OnOverlapDetected(IGhost ghost)
    {
        //Debug.Log("Overlap Detected!");
        ghost.IsOverlapDetected = true;
    }
    public void SetCurrentWave(int wave)
    {
        currentWave = wave;
        //ここをコメントアウトすれば現状のクリア条件消せる
        if (wave == 5 && !isClearCountdownStarted)
        {
            StartCoroutine(ClearCountdownCoroutine());
            isClearCountdownStarted = true;
        }
    }


    private GameObject GetRandomGhostForCurrentWave()
    {
        if (ghostSpawnInfos.Count == 0) return null;
        if (currentWave == 0) return null;

        // Wave数が1始まりなので-1（例：Wave1→index0）
        int waveIndex = Mathf.Clamp(currentWave - 1, 0, ghostSpawnInfos[0].spawnRatesPerWave.Length - 1);

        // 合計が1.0になる前提
        float randomValue = Random.value; // 0〜1
        float cumulative = 0f;

        foreach (var info in ghostSpawnInfos)
        {
            cumulative += info.spawnRatesPerWave[waveIndex];
            if (randomValue <= cumulative)
            {
                return info.ghostPrefab;
            }
        }

        // 万が一合計が1未満の場合、最後のを返す
        return ghostSpawnInfos[ghostSpawnInfos.Count - 1].ghostPrefab;
    }
    

    public void CheckGameResult()
    {
        if (currentPlayerHp <= 0)
        {
            GameOver();
        }
    }

    public void Victory()
    {
        Debug.Log("Victory");
        PhantomSwing.Instance.GameData.SetScore(currentDeadGhostCount);
        PhantomSwing.Instance.LoadGameScene("GameClearScene");
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
        PhantomSwing.Instance.LoadGameScene("GameOverScene");
    }

    private float GetDynamicSpawnTime()
    {
        // 現在のWaveに対応する設定を取得
        WaveSpawnSettings waveSetting = waveSpawnSettingsList.Find(w => w.waveNumber == currentWave);
        if (waveSetting == null)
        {
            Debug.LogWarning("Wave設定が見つかりません。デフォルト値を使用します。");
            return ghostSpawnTime; // fallback
        }

        int half = maxGhostCount / 2;
        int count = ghostsList.Count;

        // 0のときは最大のスポーン時間にする（遅く）
        if (count == 0)
        {
            return waveSetting.maxSpawnTime;
        }

        float t;

        if (count <= half)
        {
            // count が小さいほど t は大きくなる (例: count=1なら t=1, count=halfなら t=1/half)
            t = 1f / count;
        }
        else
        {
            // half超えたら0から1の範囲に正規化
            // countがhalfの時に0、maxGhostCountの時に1になるようにする
            t = (float)(count - half) / (maxGhostCount - half);
        }

        // tは0〜1の間の数値になっている想定で補間
        return Mathf.Lerp(waveSetting.minSpawnTime, waveSetting.maxSpawnTime, t);
    }

    private IEnumerator ClearCountdownCoroutine()
    {
        float timer = clearCountdownDuration;

        while (timer > 0)
        {
            if (timer <= countdownWarningStart && timer <= 5f)
            {
                uiManager.ShowCountdownText(Mathf.CeilToInt(timer)); // 5〜1の整数表示
            }

            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        Victory();
    }

    public void HealPlayer(int amount)
    {
        currentPlayerHp = Mathf.Min(currentPlayerHp + amount, maxPlayerHp);
        uiManager.SetHpPanel(currentPlayerHp);
    }

}
