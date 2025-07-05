using UnityEngine;

namespace Ghosts
{
    public class FastGhost : IGhost
    {
        public override bool GetIsAttackable(SwingDirection swingDirection, SwingSpeed speed)
        {
            if(isInAttackableRange && speed == SwingSpeed.Fast && isOverlapDetected)
                return true;
            else
            {
                return false;
            }
        }
    }
}