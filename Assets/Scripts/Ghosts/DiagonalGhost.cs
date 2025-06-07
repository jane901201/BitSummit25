using UnityEngine;

namespace Ghosts
{
    public class DiagonalGhost : IGhost
        
    {
        
        protected override void Start()
        {
            base.Start();
            gameObject.transform.rotation = Quaternion.Euler(0, 0,-37.504f);
        }
        
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