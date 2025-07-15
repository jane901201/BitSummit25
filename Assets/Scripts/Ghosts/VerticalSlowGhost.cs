namespace Ghosts
{
    public class VerticalSlowGhost : IGhost
    {
        public override bool GetIsAttackable(SwingDirection direction, SwingSpeed swingSpeed)
        {
            if(isInAttackableRange && direction == SwingDirection.Vertical && swingSpeed == SwingSpeed.Slow && isOverlapDetected)
                return true;
            else
            {
                return false;
            }
        }
    }
}