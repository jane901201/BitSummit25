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

        [SerializeField] private AttackHitBox attackHitBox; // 子のAttackHitBoxスクリプト
        [SerializeField] protected Transform playerTransform;



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

            if (playerTransform == null)
                playerTransform = GameObject.FindWithTag("Player").transform;

        } 
        
        protected virtual void Start()
        {
            currentHP = maxHP;
            if(rigidbody == null)
                rigidbody = GetComponent<Rigidbody>();

            forward = -cameraTransform.forward;
            forward.y = 0;
            forward.Normalize();

            if (attackHitBox != null)
            {
                attackHitBox.ownerGhost = this;
            }
        }

        protected virtual void FixedUpdate()
        {
            Move();
        }

        public virtual void Move()
        {
            if (isStopped)
                return;

            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.Normalize();

            // 高低差も含めてプレイヤーの方向に向かせる
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                // 正面が -Z のモデルなので 180° 回転
                targetRotation *= Quaternion.Euler(0, 180f, 0);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            // 進む方向はXZ平面に限定したほうが自然なら：
            Vector3 horizontalDirection = playerTransform.position - transform.position;
            horizontalDirection.y = 0;
            horizontalDirection.Normalize();
            rigidbody.MovePosition(rigidbody.position + horizontalDirection * moveSpeed * Time.deltaTime);

            // もし高低差方向にも進ませたいなら、こっち：
            rigidbody.MovePosition(rigidbody.position + directionToPlayer * moveSpeed * Time.deltaTime);
        }




        public virtual void AttackAnimation()
        {
            isStopped = true;
            // 攻撃アニメーション再生（必要ならアンコメント）
            // attackAnimator.SetTrigger("Attack");

            if (attackAnimator != null)
            {
                attackAnimator.SetTrigger("Attack"); // Animatorのパラメータ名を合わせる
            }


            transform.DOShakePosition(shakeDuration, shakeStrength);
        }
        
        public virtual bool GetIsAttackable(SwingDirection swingDirection, SwingSpeed swingSpeed)
        {
            return isInAttackableRange && isOverlapDetected; 
        }

        public virtual bool GetIsAttackableRange()
        {
            return isInAttackableRange;
        }
        
        public void SetIsInAttackableRange(bool isAttackable)
        {
            isInAttackableRange = isAttackable;
            //Debug.Log(isInAttackableRange);
        }
        
        public void HpBarUpdate()
        {
        }
        
        public virtual void TakeDamage(int damage)
        {
            currentHP -= damage;

            if (attackAnimator != null)
            {
                attackAnimator.SetTrigger("Damage"); // Animatorのパラメータ名を合わせる
            }

            //Debug.Log("TakeDamage");
            //ダメージ受けると置き
        }

        public virtual void Die()
        {
            //死ぬとこ
            // スコアとゲージ加算
            GameManager.Instance.AddScore(scoreValue);
            GameManager.Instance.AddGauge(gaugeValue);
            GameManager.Instance.AddCurrentDeadGhostCount();
        }
        public void SetOverlapDetected(bool value)
        {
            isOverlapDetected = value;
        }

    }
}
