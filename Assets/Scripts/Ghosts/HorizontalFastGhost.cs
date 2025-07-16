namespace Ghosts
{
    public class HorizontalFastGhost : IGhost
    {
        public override bool GetIsAttackable(SwingDirection direction, SwingSpeed swingSpeed)
        {
            if(isInAttackableRange && direction == SwingDirection.Horizontal && swingSpeed == SwingSpeed.Fast && isOverlapDetected)
                return true;
            else
            {
                return false;
            }
        }
    }
}