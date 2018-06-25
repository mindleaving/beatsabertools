using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberSongGenerator.Objects;
using Commons;
using BeatSaberSongGenerator.AudioProcessing;

namespace BeatSaberSongGenerator.Generators
{
    public class BaseRhythmGenerator
    {
        private static HorizontalPosition GetRandHorPosition()
        {
            var val = StaticRandom.Rng.Next(0,4);
            switch (val)
            {
                case 0:
                    return HorizontalPosition.CenterLeft;
                case 1:
                    return HorizontalPosition.CenterRight;
                case 2:
                    return HorizontalPosition.Left;
                case 3:
                    return HorizontalPosition.Right;
                default:
                    throw new ArgumentOutOfRangeException(nameof(val));
            }
        }

        private static VerticalPosition GetRandVertPosition()
        {
            var val = StaticRandom.Rng.Next(0,3);
            switch (val)
            {
                case 0:
                    return VerticalPosition.Bottom;
                case 1:
                    return VerticalPosition.Middle;
                case 2:
                    return VerticalPosition.Top;
                default:
                    throw new ArgumentOutOfRangeException(nameof(val));
            }
        }

        private static Hand GetRandomHand()
        {
            var val = StaticRandom.Rng.Next(0, 2);
            switch (val)
            {
                case 0:
                    return Hand.Left;
                case 1:
                    return Hand.Right;
                default:
                    throw new ArgumentOutOfRangeException(nameof(val));
            }
        }

        public List<Note> Generate(List<Beat> mergedBeats, AudioMetadata metadata)
        {
            var notes = new List<Note>();
            foreach (var beat in metadata.BeatDetectorResult.DetectedBeats)
            {
                var time = TimeConversion.SampleIndexToBeatIndex(
                    beat.SampleIndex,
                    metadata.SampleRate,
                    metadata.BeatDetectorResult.BeatsPerMinute);
                var note = new Note(time, GetRandomHand(), CutDirection.Any, GetRandHorPosition(), GetRandVertPosition());
                notes.Add(note);
            }
            /*
            var barConfiguration = Enumerable.Range(0, beatsPerBar)
                .Select(_ => GenerateRandomBeatConfiguration())
                .ToList();

            var modificationProbability = 0.2;
            var modifications = new BeatConfiguration[beatsPerBar];
            var notes = new List<Note>();
            for (var barIdx = 0; barIdx < detectedBeats.Count(); barIdx += beatsPerBar)
            {
                if (detectedBeats[barIdx].Strength == 0) continue;
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
                    var beatConfiguration = modifications[beatIdx] ?? barConfiguration[beatIdx];
                    var combination = GetCombination(barIdx + beatIdx, beatConfiguration);
                    foreach (var note in combination)
                        if(note.BeatIndex < detectedBeats.Count())
                            note.Time = (float)( bpm * (double)detectedBeats[note.BeatIndex].SampleIndex / (double)sampleCount / 60.0);
                    notes.AddRange(combination);
                }
            }
            */
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
