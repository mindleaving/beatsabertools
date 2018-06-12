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
            var barConfiguration = Enumerable.Range(0, beatsPerBar)
                .Select(_ => GenerateRandomBeatConfiguration())
                .ToList();

            var modificationProbability = 0.2;
            var modifications = new BeatConfiguration[beatsPerBar];
            var notes = new List<Note>();
            for (var barIdx = 0; barIdx < barCount; barIdx++)
            {
                if (StaticRandom.Rng.NextDouble() < modificationProbability)
                {
                    var modificationBeatIdx = StaticRandom.Rng.Next(beatsPerBar);
                    if (modifications[modificationBeatIdx] != null)
                    {
                        modifications[modificationBeatIdx] = null;
                    }
                    else
                    {
                        modifications[modificationBeatIdx] = GenerateRandomBeatConfiguration();
                    }
                }
                for (var beatIdx = 0; beatIdx < beatsPerBar; beatIdx++)
                {
                    var time = timeOffset + barIdx * beatsPerBar + beatIdx;
                    var beatConfiguration = modifications[beatIdx] ?? barConfiguration[beatIdx];
                    var combination = GetCombination(time,beatConfiguration);
                    notes.AddRange(combination);
                }
            }
            return notes;
        }

        private static BeatConfiguration GenerateRandomBeatConfiguration()
        {
            return new BeatConfiguration
            {
                CombinationIdx = StaticRandom.Rng.Next(AvailableCombinations),
                Hand = (Hand) StaticRandom.Rng.Next(2),
                HorizontalPosition = (HorizontalPosition) StaticRandom.Rng.Next(1, 3),
                VerticalPosition = (VerticalPosition) StaticRandom.Rng.Next(2)
            };
        }

        private const int AvailableCombinations = 14;
        private IEnumerable<Note> GetCombination(float time, BeatConfiguration beatConfiguration)
        {
            var combinationIndex = beatConfiguration.CombinationIdx;
            var hand = beatConfiguration.Hand;
            var horizontalPosition = beatConfiguration.HorizontalPosition;
            var verticalPosition = beatConfiguration.VerticalPosition;
            switch (combinationIndex)
            {
                case 0:
                    return NoteCombinationLibrary.DownUp(time, hand, horizontalPosition, verticalPosition);
                case 1:
                    return NoteCombinationLibrary.BlueRedStacked(time, horizontalPosition, verticalPosition);
                case 2:
                    return NoteCombinationLibrary.RedBlueStacked(time, horizontalPosition, verticalPosition);
                case 3:
                    return NoteCombinationLibrary.CrossDoubleDownDiagonal(time, verticalPosition);
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
                case 10:
                    return new[] {new Note(time, hand, CutDirection.Down, horizontalPosition, verticalPosition)};
                case 11:
                    var cutDirection = StaticRandom.Rng.Next(2) == 1 ? CutDirection.Down : CutDirection.Up;
                    return NoteCombinationLibrary.OppositeDownUp(time, cutDirection, horizontalPosition, verticalPosition);
                case 12:
                    return NoteCombinationLibrary.CrossDoubleHorizontalBlueRed(time, verticalPosition);
                case 13:
                    return NoteCombinationLibrary.CrossDoubleHorizontalRedBlue(time, verticalPosition);
                default:
                    throw new ArgumentOutOfRangeException(nameof(combinationIndex));
            }
        }
    }
}
