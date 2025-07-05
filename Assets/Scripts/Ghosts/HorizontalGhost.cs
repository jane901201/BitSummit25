namespace Ghosts
{
    public class HorizontalGhost : IGhost
    {
        public override bool GetIsAttackable(SwingDirection direction, SwingSpeed swingSpeed)
        {
            if(isInAttackableRange && direction == SwingDirection.Horizontal && isOverlapDetected)
                return true;
            else
            {
                return false;
            }
        }
    }
}