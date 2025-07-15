namespace Ghosts
{
    public class DiagonalSlowGhost : IGhost
    {
        public override bool GetIsAttackable(SwingDirection direction, SwingSpeed swingSpeed)
        {
            if(isInAttackableRange && direction == SwingDirection.Diagonal && swingSpeed == SwingSpeed.Slow && isOverlapDetected)
                return true;
            else
            {
                return false;
            }
        }
    }
}