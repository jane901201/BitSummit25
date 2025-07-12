using UnityEngine;

namespace Ghosts
{
    public class AttackHitBox : MonoBehaviour
    {
        public IGhost ownerGhost;  // 親を参照

        private void OnTriggerEnter(Collider other)
        {
            // プレイヤーの武器などが当たったときの処理例
            if (other.CompareTag("Player"))
            {
                ownerGhost.SetIsInAttackableRange(true);
                ownerGhost.SetOverlapDetected(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ownerGhost.SetIsInAttackableRange(false);
                ownerGhost.SetOverlapDetected(false);
            }
        }
    }
}

