using System;

namespace BeatSaberSongGenerator.Objects
{
    public enum CutDirection
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
        UpLeft = 4,
        UpRight = 5,
        DownLeft = 6,
        DownRight = 7,
        Any = 8
    }

    public static class CutDirectionExtensions
    {
        public static CutDirection Invert(this CutDirection cutDirection)
        {
            switch (cutDirection)
            {
                case CutDirection.Up:
                    return CutDirection.Down;
                case CutDirection.Down:
                    return CutDirection.Up;
                case CutDirection.Left:
                    return CutDirection.Right;
                case CutDirection.Right:
                    return CutDirection.Left;
                case CutDirection.UpLeft:
                    return CutDirection.DownRight;
                case CutDirection.UpRight:
                    return CutDirection.DownLeft;
                case CutDirection.DownLeft:
                    return CutDirection.UpRight;
                case CutDirection.DownRight:
                    return CutDirection.UpLeft;
                case CutDirection.Any:
                    return CutDirection.Any;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cutDirection), cutDirection, null);
            }
        }
    }
}