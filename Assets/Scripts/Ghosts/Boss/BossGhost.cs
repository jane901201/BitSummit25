using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Ghosts
{
    //TODO:Hp:600
    public class BossGhost : IGhost
    {
        //摸行本身的選轉，到了定點更改方向
        //初始方向
        //要重新處理死亡的動畫消失時間

        [SerializeField] private GameObject body;
        [SerializeField] private GameObject handRight;
        [SerializeField] private GameObject handLeft;
        [SerializeField] private List<GameObject> points;
        [SerializeField] private List<BossGhostPart> parts;
        public RotationDirection CurrentRotationDirection { get; set; }
        public List<IGhost> BossPartsGhosts;
        
        private Vector3 leftHandOriginalPos;
        private Vector3 rightHandOriginalPos;
        
        private void Awake()
        {
            parts.ForEach(part => part.BossGhost = this);
            leftHandOriginalPos = handLeft.transform.localPosition;
            rightHandOriginalPos = handRight.transform.localPosition;
            BossPartsGhosts = new List<IGhost>(parts);
        }

        private void Start()
        {
            //AttackAnimation();
            IdleAnimation();
        }
        

        private void AttackAnimation()
        {
            //手的動畫
            // 後ろ上に準備位置（例として y+0.2, z-0.2）
            Vector3 leftReadyPos = handLeft.transform.position + new Vector3(0, 0.2f, 0.2f);
            Vector3 rightReadyPos = handRight.transform.position + new Vector3(0, 0.2f, 0.2f);

            Vector3 leftAttackPos = handLeft.transform.position + new Vector3(0, 0, -0.5f);
            Vector3 rightAttackPos = handRight.transform.position + new Vector3(0, 0, -0.5f);;
            
            Sequence leftSeq = DOTween.Sequence();
            leftSeq.Append(handLeft.transform.DOLocalMove(leftReadyPos, 0.2f))
                .Append(handLeft.transform.DOMove(leftAttackPos, 0.3f))
                .Append(handLeft.transform.DOLocalMove(leftHandOriginalPos, 0.2f));
            
            Sequence rightSeq = DOTween.Sequence();
            rightSeq.Append(handRight.transform.DOLocalMove(rightReadyPos, 0.2f))
                .Append(handRight.transform.DOMove(rightAttackPos, 0.3f))
                .Append(handRight.transform.DOLocalMove(rightHandOriginalPos, 0.2f));
            
            //Boss的動畫
            CurrentRotationDirection = RotationDirection.CounterClockwise;
            // 最初の-90度回転（1秒）
            Sequence seq = DOTween.Sequence();
            seq.Append(body.transform.DORotate(new Vector3(0, 0, -90), 1f, RotateMode.FastBeyond360));

            // 停止（0.2秒）
            seq.AppendInterval(0.2f);

            CurrentRotationDirection = RotationDirection.Clockwise;
            // 時計回りに360度（普通速度）
            seq.Append(body.transform.DORotate(new Vector3(0, 0, 270), 0.5f, RotateMode.FastBeyond360));

            // 時計回りに360度（緩やかに、Ease使用）
            seq.Append(body.transform.DORotate(new Vector3(0, 0, 630), 0.5f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad));

        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
        }

        public override bool GetIsAttackableRange()
        {
            return false;
        }

        public override bool GetIsAttackable(SwingDirection swingDirection, SwingSpeed swingSpeed)
        {
            return false;
        }

        private void IdleAnimation()
        {
            // 上下ふよふよ動くアニメーション
            body.transform.DOMoveY(body.transform.position.y + 0.5f, 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
            
            handRight.transform.DOMoveY(handRight.transform.position.y + 0.5f, 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            handLeft.transform.DOMoveY(handLeft.transform.position.y + 0.5f, 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            // ゆっくり回転（10秒で1回転）
            body.transform.DORotate(new Vector3(0, 0, 360), 10f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }

        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.K))
            // {
            //     parts[0].gameObject.SetActive(false);
            //     //Debug.Log("dead");
            // }
            if(IsDead())
                return;
        }

        public override void Move()
        {
        }
    }
    
    public enum RotationDirection
    {
        Clockwise,
        CounterClockwise
    }
}