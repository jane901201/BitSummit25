using UnityEngine;

namespace Ghosts
{
    public class HealGhost : IGhost
    {
        [SerializeField] private bool canHealPlayer = true;

        public override void Die()
        {
            base.Die(); // 通常の死亡処理（スコア加算やエフェクトなど）

            if (canHealPlayer)
            {
                GameManager.Instance.HealPlayer(1);
            }
        }
    }
}

