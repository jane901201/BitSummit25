using DG.Tweening;
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

        [SerializeField] protected Animator attackAnimator;
        [SerializeField] protected float shakeDuration = 0.5f;
        [SerializeField] protected float shakeStrength = 0.2f;
        [SerializeField] protected float destroyDelay = 1f;

        private bool hasAttacked = false;
        private bool isStopped = false;


        
        //TODO:HPBar 可以直接掛在角色身上嗎? 
        private Vector3 forward;
        protected bool isOverlapDetected;
        
        public int GetAttackPower
        {
            get
            {
                if (!hasAttacked)
                {
                    hasAttacked = true;
                    return attackPower;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int GetHp() => currentHP;
        public bool IsDead() => currentHP <= 0;
        public bool IsOverlapDetected
        {
            get => isOverlapDetected;
            set => isOverlapDetected = value;
        }
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
            if (isStopped)
                return;
            rigidbody.MovePosition(rigidbody.position + forward * moveSpeed * Time.deltaTime);
        }

        public virtual void AttackAnimation()
        {
            isStopped = true;
            // 攻撃アニメーション再生（必要ならアンコメント）
            // attackAnimator.SetTrigger("Attack");

            // DOTweenで震えた後、1秒後にオブジェクトを削除
            transform.DOShakePosition(shakeDuration, shakeStrength);
                // .OnComplete(() =>
                // {
                //     // 震えが終わってから1秒待って破棄
                //     DOVirtual.DelayedCall(destroyDelay, () =>
                //     {
                //         GameManager.Instance.RemoveGhost(this);
                //     });
                // });
        }
        
        public virtual bool GetIsAttackable(SwingDirection swingDirection, SwingSpeed swingSpeed)
        {
            return isInAttackableRange && isOverlapDetected; 
        }
        
        public void SetIsInAttackableRange(bool isAttackable)
        {
            isInAttackableRange = isAttackable;
            //Debug.Log(isInAttackableRange);
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
