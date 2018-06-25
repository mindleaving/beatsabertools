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
        private Random rand = new Random();

        HorizontalPosition GetRandHorPosition()
        {
            int val = rand.Next(0,4);
            if (val == 0) return HorizontalPosition.CenterLeft;
            if (val == 1) return HorizontalPosition.CenterRight;
            if (val == 2) return HorizontalPosition.Left;
            if (val == 3) return HorizontalPosition.Right;
            return HorizontalPosition.Right;
        }

        VerticalPosition GetRandVertPosition()
        {
            int val = rand.Next(0,3);
            if (val == 0) return VerticalPosition.Bottom;
            if (val == 1) return VerticalPosition.Middle;
            if (val == 2) return VerticalPosition.Top;
            return VerticalPosition.Bottom;
        }

        Hand GetRandomHand()
        {
            int val = rand.Next(0, 2);
            if (val == 0) return Hand.Left;
            if (val == 1) return Hand.Right;
            return Hand.Left;
        }

        public List<Note> Generate(List<Beat> mergedBeats, AudioMetadata metadata)
        {
            var notes = new List<Note>();
            for (var i = 0; i < metadata.BeatDetectorResult.DetectedBeats.Count(); ++i)
            {
                Note n = new Note(i, Hand.Left, CutDirection.Any, HorizontalPosition.CenterLeft, VerticalPosition.Middle);
                n.HorizontalPosition = GetRandHorPosition();
                n.VerticalPosition = GetRandVertPosition();
                n.Hand = GetRandomHand();
                n.Time = (float)(metadata.BeatDetectorResult.BeatsPerMinute * (double)metadata.BeatDetectorResult.DetectedBeats[i].SampleIndex / (double)metadata.SampleRate / 60.0);
                notes.Add(n);
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
        private IEnumerable<Note> GetCombination(int beatIndex, BeatConfiguration beatConfiguration)
        {
            var combinationIndex = beatConfiguration.CombinationIdx;
            var hand = beatConfiguration.Hand;
            var horizontalPosition = beatConfiguration.HorizontalPosition;
            var verticalPosition = beatConfiguration.VerticalPosition;
            switch (combinationIndex)
            {
                case 0:
                    return NoteCombinationLibrary.DownUp(beatIndex, hand, horizontalPosition, verticalPosition);
                case 1:
                    return NoteCombinationLibrary.BlueRedStacked(beatIndex, horizontalPosition, verticalPosition);
                case 2:
                    return NoteCombinationLibrary.RedBlueStacked(beatIndex, horizontalPosition, verticalPosition);
                case 3:
                    return NoteCombinationLibrary.CrossDoubleDownDiagonal(beatIndex, verticalPosition);
                case 4:
                    return NoteCombinationLibrary.DiagonalDownDouble(beatIndex, verticalPosition);
                case 5:
                    return NoteCombinationLibrary.DoubleDownUp(beatIndex, horizontalPosition, verticalPosition);
                case 6:
                    return NoteCombinationLibrary.LeftRight(beatIndex, horizontalPosition, verticalPosition);
                case 7:
                    return NoteCombinationLibrary.RightLeft(beatIndex, horizontalPosition, verticalPosition);
                case 8:
                    return NoteCombinationLibrary.Double(beatIndex, CutDirection.Down, horizontalPosition, verticalPosition);
                case 9:
                    return NoteCombinationLibrary.ReversedDouble(beatIndex, CutDirection.Down, horizontalPosition, verticalPosition);
                case 10:
                    return new[] {new Note(beatIndex, hand, CutDirection.Down, horizontalPosition, verticalPosition)};
                case 11:
                    var cutDirection = StaticRandom.Rng.Next(2) == 1 ? CutDirection.Down : CutDirection.Up;
                    return NoteCombinationLibrary.OppositeDownUp(beatIndex, cutDirection, horizontalPosition, verticalPosition);
                case 12:
                    return NoteCombinationLibrary.CrossDoubleHorizontalBlueRed(beatIndex, verticalPosition);
                case 13:
                    return NoteCombinationLibrary.CrossDoubleHorizontalRedBlue(beatIndex, verticalPosition);
                default:
                    throw new ArgumentOutOfRangeException(nameof(combinationIndex));
            }
        }
    }
}
