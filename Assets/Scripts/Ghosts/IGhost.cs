using Unity.VisualScripting;
using UnityEngine;

namespace Ghosts
{
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
        [SerializeField] protected bool isInAttackableRange; 
        [SerializeField] protected Sprite attackableIcon;
        //TODO:HPBar 可以直接掛在角色身上嗎? 
        private Vector3 forward;
        
        public int GetAttackPower() => attackPower;
        public int GetHp() => currentHP;
        public bool IsDead() => currentHP <= 0;
        public bool GetIsInAttackableRange() => isInAttackableRange;

        protected void Awake()
        {
            if(cameraTransform == null)
                cameraTransform = Camera.main.transform;
            
        } 
        
        protected virtual void Start()
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


        public virtual bool GetIsAttackable(SwingDirection swingDirection, SwingSpeed swingSpeed)
        {
            return isInAttackableRange; 
        }

        public void SetIsInAttackableRange(bool isAttackable)
        {
            isInAttackableRange = isAttackable;
            //Debug.Log(isAttackable);
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
            //Debug.Log("TakeDamage");
        }

        public void Die()
        {
            // スコアとゲージ加算
            GameManager.Instance.AddScore(scoreValue);
            GameManager.Instance.AddGauge(gaugeValue);
            GameManager.Instance.AddCurrentDeadGhostCount();
        }
    }
    
}
