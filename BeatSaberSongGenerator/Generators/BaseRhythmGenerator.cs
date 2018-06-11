using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberSongGenerator.Objects;
using Commons;

namespace BeatSaberSongGenerator.Generators
{
    public class BaseRhythmGenerator
    {
        public List<Note> Generate(int beatsPerBar, int barCount, float timeOffset)
        {
            var combinationIndices = Enumerable.Range(0, beatsPerBar)
                .Select(_ => new
                {
                    CombinationIdx = StaticRandom.Rng.Next(AvailableCombinations),
                    Hand = (Hand)StaticRandom.Rng.Next(2),
                    HorizontalPosition = (HorizontalPosition)StaticRandom.Rng.Next(1, 3),
                    VerticalPosition = (VerticalPosition)StaticRandom.Rng.Next(2)
                })
                .ToList();
            var notes = new List<Note>();
            for (int barIdx = 0; barIdx < barCount; barIdx++)
            {
                for (int beatIdx = 0; beatIdx < beatsPerBar; beatIdx++)
                {
                    var time = timeOffset + barIdx * beatsPerBar + beatIdx;
                    var args = combinationIndices[beatIdx];
                    var combination = GetCombination(args.CombinationIdx, time, args.Hand, args.HorizontalPosition, args.VerticalPosition);
                    notes.AddRange(combination);
                }
            }
            return notes;
        }

        private const int AvailableCombinations = 10;
        private IEnumerable<Note> GetCombination(
            int combinationIndex,
            float time,
            Hand hand,
            HorizontalPosition horizontalPosition,
            VerticalPosition verticalPosition)
        {
            switch (combinationIndex)
            {
                case 0:
                    return NoteCombinationLibrary.DownUp(time, hand, horizontalPosition, verticalPosition);
                case 1:
                    return NoteCombinationLibrary.BlueRedStacked(time, horizontalPosition, verticalPosition);
                case 2:
                    return NoteCombinationLibrary.RedBlueStacked(time, horizontalPosition, verticalPosition);
                case 3:
                    return NoteCombinationLibrary.CrossDouble(time, verticalPosition);
                case 4:
                    return NoteCombinationLibrary.DiagonalDownDouble(time, verticalPosition);
                case 5:
                    return NoteCombinationLibrary.DoubleDownUp(time, horizontalPosition, verticalPosition);
                case 6:
                    return NoteCombinationLibrary.LeftRight(time, horizontalPosition, verticalPosition);
                case 7:
                    return NoteCombinationLibrary.RightLeft(time, horizontalPosition, verticalPosition);
                case 8:
                    return NoteCombinationLibrary.Double(time, CutDirection.Down, horizontalPosition, verticalPosition);
                case 9:
                    return NoteCombinationLibrary.ReversedDouble(time, CutDirection.Down, horizontalPosition, verticalPosition);
                default:
                    throw new ArgumentOutOfRangeException(nameof(combinationIndex));
            }
        }
    }
}
