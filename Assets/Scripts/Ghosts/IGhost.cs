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

        [SerializeField] protected float floatAmplitude = 0.5f; // 上下移動の振れ幅
        [SerializeField] protected float floatFrequency = 1f;   // 上下移動の速さ

        private float floatTimer = 0f;
        private Vector3 initialPosition;


        [SerializeField] private bool shouldTiltZ = false;
        [SerializeField] private float tiltZRotation = -37.504f;


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
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody>();

            forward = -cameraTransform.forward;
            forward.y = 0;
            forward.Normalize();

            if (attackHitBox != null)
            {
                attackHitBox.ownerGhost = this;
            }

            initialPosition = transform.position; // ←追加した行
        }


        protected virtual void Update()
        {
            //FloatMotion();
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

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                targetRotation *= Quaternion.Euler(0, 180f, 0);

                // Z軸に傾けたい場合だけ追加
                if (shouldTiltZ)
                {
                    targetRotation *= Quaternion.Euler(0, 0, tiltZRotation);
                }

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            // 進行方向（XZ or 高低含む）
            Vector3 horizontalDirection = playerTransform.position - transform.position;
            horizontalDirection.y = 0;
            horizontalDirection.Normalize();

            // 必要に応じて下だけ残す
            rigidbody.MovePosition(rigidbody.position + horizontalDirection * moveSpeed * Time.deltaTime);
        }



        public virtual void AttackAnimation()
        {
            Vector3 originalPosition = transform.position;

            transform.SetParent(GameManager.Instance.ghostGroup.transform, true);

            transform.position = originalPosition; // 位置を固定する

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

        private void FloatMotion()
        {
            floatTimer += Time.deltaTime * floatFrequency;
            float newY = Mathf.Sin(floatTimer) * floatAmplitude;

            Vector3 currentPosition = rigidbody.position;
            currentPosition.y = initialPosition.y + newY;

            rigidbody.MovePosition(currentPosition); // Rigidbodyを使う！
        }


    }
}
