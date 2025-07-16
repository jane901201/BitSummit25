namespace Ghosts
{
    public class VerticalFastGhost : IGhost
    {
        public override bool GetIsAttackable(SwingDirection direction, SwingSpeed swingSpeed)
        {
            if(isInAttackableRange && direction == SwingDirection.Vertical && swingSpeed == SwingSpeed.Fast && isOverlapDetected)
                return true;
            else
            {
                return false;
            }
        }
    }
}