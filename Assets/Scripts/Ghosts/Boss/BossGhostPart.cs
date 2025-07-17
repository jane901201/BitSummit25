using System;
using UnityEngine;

namespace Ghosts
{
    //把被攻擊的資料傳回BossGhost
    //死掉的話
    public class BossGhostPart : IGhost
    {
        [SerializeField] private SwingInfo currentDirectionInfo = new SwingInfo(SwingDirection.Horizontal, SwingSubDirection.None);
        [SerializeField] private SwingSpeed currentSpeed = SwingSpeed.Fast;
        [SerializeField] private Transform checkPoint;

        
        public BossGhost BossGhost { get; set; }
        
        public SwingInfo CurrentDirection
        {
            get { return currentDirectionInfo;}
            set { currentDirectionInfo = value; }
        } 
        
        public void Update()
        {
            //Debug.Log(transform.position);
        }

        public override void TakeDamage(int damage)
        {
            BossGhost.TakeDamage(damage);
            base.TakeDamage(damage);
        }

        public override bool GetIsAttackableRange()
        {
            return true;
        }

        public override bool GetIsAttackable(SwingDirection direction, SwingSpeed swingSpeed)
        {
            if(direction == currentDirectionInfo.Base)
                return true;
            else
            {
                return false;
            }
        }

    }
}