using UnityEngine;

namespace Ghosts
{
    public class SlowGhost: IGhost
    {
        public override bool GetIsAttackable(SwingDirection swingDirection, SwingSpeed speed)
        {
            if (isInAttackableRange && speed == SwingSpeed.Slow)
                return true;
            else
            {
                return false;
            }
        }
    }
}