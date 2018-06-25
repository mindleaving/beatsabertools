using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberSongGenerator.Objects;
using Commons.Extensions;

namespace BeatSaberSongGenerator.Generators
{
    public static class NoteCombinationLibrary
    {
        public static IEnumerable<Note> DownUp(
            float time, 
            Hand hand,
            HorizontalPosition horizontalPosition,
            VerticalPosition verticalPosition)
        {
            return new []
            {
                new Note(time, hand, CutDirection.Down, horizontalPosition, verticalPosition), 
                new Note(time + 0.5f, hand, CutDirection.Up, horizontalPosition, verticalPosition)
            };
        }

        public static IEnumerable<Note> OppositeDownUp(
            float time, 
            CutDirection leftCutDirection,
            HorizontalPosition leftHorizontalPosition,
            VerticalPosition verticalPosition)
        {
            if(leftHorizontalPosition == HorizontalPosition.Right)
                throw new ArgumentOutOfRangeException(nameof(leftHorizontalPosition));
            return new []
            {
                new Note(time, Hand.Left, leftCutDirection, leftHorizontalPosition, verticalPosition), 
                new Note(time, Hand.Right, leftCutDirection.Invert(), leftHorizontalPosition+1, verticalPosition)
            };
        }

        public static IEnumerable<Note> DoubleDownUp(
            float time, 
            HorizontalPosition leftHorizontalPosition,
            VerticalPosition verticalPosition)
        {
            if(leftHorizontalPosition == HorizontalPosition.Right)
                throw new ArgumentOutOfRangeException(nameof(leftHorizontalPosition));
            return DownUp(time, Hand.Left, leftHorizontalPosition, verticalPosition)
                .Concat(DownUp(time, Hand.Right, leftHorizontalPosition + 1, verticalPosition))
                .OrderBy(x => x.Time);
        }

        public static IEnumerable<Note> Double(
            float time, 
            CutDirection cutDirection,
            HorizontalPosition leftHorizontalPosition,
            VerticalPosition verticalPosition)
        {
            if(leftHorizontalPosition == HorizontalPosition.Right)
                throw new ArgumentOutOfRangeException(nameof(leftHorizontalPosition));
            return new[]
            {
                new Note(time, Hand.Left, cutDirection, leftHorizontalPosition, verticalPosition),
                new Note(time, Hand.Right, cutDirection, leftHorizontalPosition + 1, verticalPosition)
            };
        }

        public static IEnumerable<Note> ReversedDouble(
            float time, 
            CutDirection cutDirection,
            HorizontalPosition leftHorizontalPosition,
            VerticalPosition verticalPosition)
        {
            if(leftHorizontalPosition == HorizontalPosition.Left)
                throw new ArgumentOutOfRangeException(nameof(leftHorizontalPosition));
            return new[]
            {
                new Note(time, Hand.Left, cutDirection, leftHorizontalPosition, verticalPosition),
                new Note(time, Hand.Right, cutDirection, leftHorizontalPosition - 1, verticalPosition)
            };
        }

        public static IEnumerable<Note> CrossDoubleDownDiagonal(
            float time, 
            VerticalPosition verticalPosition)
        {
            return new[]
            {
                new Note(time, Hand.Left, CutDirection.DownRight, HorizontalPosition.CenterLeft, verticalPosition),
                new Note(time, Hand.Right, CutDirection.DownLeft, HorizontalPosition.CenterRight, verticalPosition)
            };
        }

        public static IEnumerable<Note> CrossDoubleHorizontalRedBlue(
            float time, 
            VerticalPosition verticalPosition)
        {
            if(verticalPosition == VerticalPosition.Top)
                throw new ArgumentOutOfRangeException(nameof(verticalPosition));
            return new[]
            {
                new Note(time, Hand.Left, CutDirection.Right, HorizontalPosition.CenterLeft, verticalPosition+1),
                new Note(time, Hand.Right, CutDirection.Left, HorizontalPosition.CenterRight, verticalPosition)
            };
        }

        public static IEnumerable<Note> CrossDoubleHorizontalBlueRed(
            float time, 
            VerticalPosition verticalPosition)
        {
            if(verticalPosition == VerticalPosition.Top)
                throw new ArgumentOutOfRangeException(nameof(verticalPosition));
            return new[]
            {
                new Note(time, Hand.Left, CutDirection.Right, HorizontalPosition.CenterLeft, verticalPosition),
                new Note(time, Hand.Right, CutDirection.Left, HorizontalPosition.CenterRight, verticalPosition+1)
            };
        }

        public static IEnumerable<Note> DiagonalDownDouble(
            float time, 
            VerticalPosition verticalPosition)
        {
            return new[]
            {
                new Note(time, Hand.Left, CutDirection.DownLeft, HorizontalPosition.Left, verticalPosition),
                new Note(time, Hand.Right, CutDirection.DownRight, HorizontalPosition.Right, verticalPosition)
            };
        }

        public static IEnumerable<Note> RedBlueStacked(
            float time,
            HorizontalPosition horizontalPosition,
            VerticalPosition lowVerticalPosition)
        {
            if(lowVerticalPosition == VerticalPosition.Top)
                throw new ArgumentOutOfRangeException(nameof(lowVerticalPosition));
            var cutDirection = horizontalPosition.InSet(HorizontalPosition.Left, HorizontalPosition.CenterLeft)
                ? CutDirection.Left
                : CutDirection.Right;
            return new[]
            {
                new Note(time, Hand.Left, cutDirection, horizontalPosition, lowVerticalPosition + 1),
                new Note(time, Hand.Right, cutDirection, horizontalPosition, lowVerticalPosition)
            };
        }

        public static IEnumerable<Note> BlueRedStacked(
            float time,
            HorizontalPosition horizontalPosition,
            VerticalPosition lowVerticalPosition)
        {
            if(lowVerticalPosition == VerticalPosition.Top)
                throw new ArgumentOutOfRangeException(nameof(lowVerticalPosition));
            var cutDirection = horizontalPosition.InSet(HorizontalPosition.Left, HorizontalPosition.CenterLeft)
                ? CutDirection.Left
                : CutDirection.Right;
            return new[]
            {
                new Note(time, Hand.Left, cutDirection, horizontalPosition, lowVerticalPosition),
                new Note(time, Hand.Right, cutDirection, horizontalPosition, lowVerticalPosition+1)
            };
        }

        public static IEnumerable<Note> RightLeft(
            float time,
            HorizontalPosition leftHorizontalPosition,
            VerticalPosition verticalPosition)
        {
            if(leftHorizontalPosition == HorizontalPosition.Right)
                throw new ArgumentOutOfRangeException(nameof(leftHorizontalPosition));
            return new[]
            {
                new Note(time, Hand.Right, CutDirection.Down, leftHorizontalPosition+1, verticalPosition),
                new Note(time+0.5f, Hand.Left, CutDirection.Down, leftHorizontalPosition, verticalPosition)
            };
        }

        public static IEnumerable<Note> LeftRight(
            float time,
            HorizontalPosition leftHorizontalPosition,
            VerticalPosition verticalPosition)
        {
            if(leftHorizontalPosition == HorizontalPosition.Right)
                throw new ArgumentOutOfRangeException(nameof(leftHorizontalPosition));
            return new[]
            {
                new Note(time, Hand.Left, CutDirection.Down, leftHorizontalPosition, verticalPosition),
                new Note(time+0.5f, Hand.Right, CutDirection.Down, leftHorizontalPosition+1, verticalPosition)
            };
        }
    }
}
