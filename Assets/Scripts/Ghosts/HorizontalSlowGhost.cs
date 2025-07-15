namespace Ghosts
{
    public class HorizontalSlowGhost : IGhost
    {
        public override bool GetIsAttackable(SwingDirection direction, SwingSpeed swingSpeed)
        {
            if(isInAttackableRange && direction == SwingDirection.Horizontal && swingSpeed == SwingSpeed.Slow && isOverlapDetected)
                return true;
            else
            {
                return false;
            }
        }
    }
}