namespace Ghosts
{
    public class VerticalGhost : IGhost
    {
        public override bool GetIsAttackable(SwingDirection direction, SwingSpeed swingSpeed)
        {
            if(isInAttackableRange && direction == SwingDirection.Vertical && isOverlapDetected)
                return true;
            else
            {
                return false;
            }
        }
    }
}