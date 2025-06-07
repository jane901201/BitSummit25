using UnityEngine;

namespace Ghosts
{
    public class DiagonalGhost : IGhost
        
    {
        public override bool GetIsAttackable(SwingDirection direction, SwingSpeed swingSpeed)
        {
            if(isInAttackableRange && direction == SwingDirection.Diagonal)
                return true;
            else
            {
                return false;
            }
        }
    }
}