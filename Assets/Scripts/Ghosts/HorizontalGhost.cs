namespace Ghosts
{
    public class HorizontalGhost : IGhost
    {
        public override bool GetIsAttackable(SwingDirection direction, SwingSpeed swingSpeed)
        {
            if(isInAttackableRange && direction == SwingDirection.Horizontal)
                return true;
            else
            {
                return false;
            }
        }
    }
}