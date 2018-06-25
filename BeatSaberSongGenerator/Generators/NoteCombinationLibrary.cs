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
            int beatIndex, 
            Hand hand,
            HorizontalPosition horizontalPosition,
            VerticalPosition verticalPosition)
        {
            return new []
            {
                new Note(beatIndex, hand, CutDirection.Down, horizontalPosition, verticalPosition), 
                new Note(beatIndex+1, hand, CutDirection.Up, horizontalPosition, verticalPosition)
            };
        }

        public static IEnumerable<Note> OppositeDownUp(
            int beatIndex,
            CutDirection leftCutDirection,
            HorizontalPosition leftHorizontalPosition,
            VerticalPosition verticalPosition)
        {
            if(leftHorizontalPosition == HorizontalPosition.Right)
                throw new ArgumentOutOfRangeException(nameof(leftHorizontalPosition));
            return new []
            {
                new Note(beatIndex, Hand.Left, leftCutDirection, leftHorizontalPosition, verticalPosition), 
                new Note(beatIndex, Hand.Right, leftCutDirection.Invert(), leftHorizontalPosition+1, verticalPosition)
            };
        }

        public static IEnumerable<Note> DoubleDownUp(
            int beatIndex,
            HorizontalPosition leftHorizontalPosition,
            VerticalPosition verticalPosition)
        {
            if(leftHorizontalPosition == HorizontalPosition.Right)
                throw new ArgumentOutOfRangeException(nameof(leftHorizontalPosition));
            return DownUp(beatIndex, Hand.Left, leftHorizontalPosition, verticalPosition)
                .Concat(DownUp(beatIndex, Hand.Right, leftHorizontalPosition + 1, verticalPosition))
                .OrderBy(x => x.Time);
        }

        public static IEnumerable<Note> Double(
            int beatIndex,
            CutDirection cutDirection,
            HorizontalPosition leftHorizontalPosition,
            VerticalPosition verticalPosition)
        {
            if(leftHorizontalPosition == HorizontalPosition.Right)
                throw new ArgumentOutOfRangeException(nameof(leftHorizontalPosition));
            return new[]
            {
                new Note(beatIndex, Hand.Left, cutDirection, leftHorizontalPosition, verticalPosition),
                new Note(beatIndex, Hand.Right, cutDirection, leftHorizontalPosition + 1, verticalPosition)
            };
        }

        public static IEnumerable<Note> ReversedDouble(
            int beatIndex,
            CutDirection cutDirection,
            HorizontalPosition leftHorizontalPosition,
            VerticalPosition verticalPosition)
        {
            if(leftHorizontalPosition == HorizontalPosition.Left)
                throw new ArgumentOutOfRangeException(nameof(leftHorizontalPosition));
            return new[]
            {
                new Note(beatIndex, Hand.Left, cutDirection, leftHorizontalPosition, verticalPosition),
                new Note(beatIndex, Hand.Right, cutDirection, leftHorizontalPosition - 1, verticalPosition)
            };
        }

        public static IEnumerable<Note> CrossDoubleDownDiagonal(
            int beatIndex,
            VerticalPosition verticalPosition)
        {
            return new[]
            {
                new Note(beatIndex, Hand.Left, CutDirection.DownRight, HorizontalPosition.CenterLeft, verticalPosition),
                new Note(beatIndex, Hand.Right, CutDirection.DownLeft, HorizontalPosition.CenterRight, verticalPosition)
            };
        }

        public static IEnumerable<Note> CrossDoubleHorizontalRedBlue(
            int beatIndex,
            VerticalPosition verticalPosition)
        {
            if(verticalPosition == VerticalPosition.Top)
                throw new ArgumentOutOfRangeException(nameof(verticalPosition));
            return new[]
            {
                new Note(beatIndex, Hand.Left, CutDirection.Right, HorizontalPosition.CenterLeft, verticalPosition+1),
                new Note(beatIndex, Hand.Right, CutDirection.Left, HorizontalPosition.CenterRight, verticalPosition)
            };
        }

        public static IEnumerable<Note> CrossDoubleHorizontalBlueRed(
            int beatIndex,
            VerticalPosition verticalPosition)
        {
            if(verticalPosition == VerticalPosition.Top)
                throw new ArgumentOutOfRangeException(nameof(verticalPosition));
            return new[]
            {
                new Note(beatIndex, Hand.Left, CutDirection.Right, HorizontalPosition.CenterLeft, verticalPosition),
                new Note(beatIndex, Hand.Right, CutDirection.Left, HorizontalPosition.CenterRight, verticalPosition+1)
            };
        }

        public static IEnumerable<Note> DiagonalDownDouble(
            int beatIndex,
            VerticalPosition verticalPosition)
        {
            return new[]
            {
                new Note(beatIndex, Hand.Left, CutDirection.DownLeft, HorizontalPosition.Left, verticalPosition),
                new Note(beatIndex, Hand.Right, CutDirection.DownRight, HorizontalPosition.Right, verticalPosition)
            };
        }

        public static IEnumerable<Note> RedBlueStacked(
            int beatIndex,
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
                new Note(beatIndex, Hand.Left, cutDirection, horizontalPosition, lowVerticalPosition + 1),
                new Note(beatIndex, Hand.Right, cutDirection, horizontalPosition, lowVerticalPosition)
            };
        }

        public static IEnumerable<Note> BlueRedStacked(
            int beatIndex,
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
                new Note(beatIndex, Hand.Left, cutDirection, horizontalPosition, lowVerticalPosition),
                new Note(beatIndex, Hand.Right, cutDirection, horizontalPosition, lowVerticalPosition+1)
            };
        }

        public static IEnumerable<Note> RightLeft(
            int beatIndex,
            HorizontalPosition leftHorizontalPosition,
            VerticalPosition verticalPosition)
        {
            if(leftHorizontalPosition == HorizontalPosition.Right)
                throw new ArgumentOutOfRangeException(nameof(leftHorizontalPosition));
            return new[]
            {
                new Note(beatIndex, Hand.Right, CutDirection.Down, leftHorizontalPosition+1, verticalPosition),
                new Note(beatIndex+1, Hand.Left, CutDirection.Down, leftHorizontalPosition, verticalPosition)
            };
        }

        public static IEnumerable<Note> LeftRight(
            int beatIndex,
            HorizontalPosition leftHorizontalPosition,
            VerticalPosition verticalPosition)
        {
            if(leftHorizontalPosition == HorizontalPosition.Right)
                throw new ArgumentOutOfRangeException(nameof(leftHorizontalPosition));
            return new[]
            {
                new Note(beatIndex, Hand.Left, CutDirection.Down, leftHorizontalPosition, verticalPosition),
                new Note(beatIndex+1, Hand.Right, CutDirection.Down, leftHorizontalPosition+1, verticalPosition)
            };
        }
    }
}
