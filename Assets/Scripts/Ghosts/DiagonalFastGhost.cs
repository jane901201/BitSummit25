namespace Ghosts
{
    public class DiagonalFastGhost : IGhost
    {
        public override bool GetIsAttackable(SwingDirection direction, SwingSpeed swingSpeed)
        {
            if(isInAttackableRange && direction == SwingDirection.Diagonal && swingSpeed == SwingSpeed.Fast && isOverlapDetected)
                return true;
            else
            {
                return false;
            }
        }
    }
}