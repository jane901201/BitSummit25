using Unity.VisualScripting;
using UnityEngine;

public class IGhost : MonoBehaviour
{
    [SerializeField] protected int maxHP = 100;
    [SerializeField] protected int currentHP;
    [SerializeField] protected int scoreValue = 10;
    [SerializeField] protected int gaugeValue = 5;
    [SerializeField] protected int attackPower = 10;
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] protected Rigidbody rigidbody;
    [Tooltip("プレイヤーに攻撃されることができる")]
    [SerializeField] protected bool isAttackable; 
    [SerializeField] protected Sprite attackableIcon;
    [Tooltip("特殊攻撃のみ有効")]
    [SerializeField] protected bool isSpecialAttackRequired;
    //TODO:HPBar 可以直接掛在角色身上嗎? 
    private Vector3 forward;
    
    public int GetAttackPower() => attackPower;

    private void Awake()
    {
        if(cameraTransform == null)
            cameraTransform = Camera.main.transform;
    } 
    
    private void Start()
    {
        currentHP = maxHP;
        if(rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();

        forward = -cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

    }

    private void FixedUpdate()
    {
        Move();
    }

    public virtual void Move()
    {
        rigidbody.MovePosition(rigidbody.position + forward * moveSpeed * Time.deltaTime);
    }

    public void SetIsAttackable(bool isAttackable)
    {
        isAttackable = isAttackable;
        Debug.Log(isAttackable);
    }
    
    public void SetAttackableIcon(Sprite attackableIcon, bool isActive)
    {
        if(isActive)
            this.attackableIcon = attackableIcon;
        attackableIcon.GameObject().SetActive(isActive);
    }
    
    public void HpBarUpdate()
    {
    }
    
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // スコアとゲージ加算
        GameManager.Instance.AddScore(scoreValue);
        GameManager.Instance.AddGauge(gaugeValue);
        
        // 自身を削除
        Destroy(gameObject);
    }
}
