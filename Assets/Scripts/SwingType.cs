using System;

public enum SwingSpeed
{
    Fast,
    Slow
}

public enum SwingDirection
{
    Vertical,
    Horizontal,
    Diagonal,
}

public enum SwingSubDirection
{
    None,
    Right,
    Left,
    Up,
    Down,
    UpRight,
    UpLeft,
    DownRight,
    DownLeft
}

[Serializable]
public struct SwingInfo
{
    public SwingDirection Base;
    public SwingSubDirection Sub;

    public SwingInfo(SwingDirection baseDir, SwingSubDirection subDir)
    {
        Base = baseDir;
        Sub = subDir;
    }

    public override bool Equals(object obj)
    {
        return obj is SwingInfo other &&
               Base == other.Base &&
               Sub == other.Sub;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Base, Sub);
    }
}