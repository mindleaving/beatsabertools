using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberSongGenerator.AudioProcessing;
using BeatSaberSongGenerator.Objects;
using Commons;
using Commons.Extensions;
using Commons.Mathematics;

namespace BeatSaberSongGenerator.Generators
{
    public class HandMovementGenerator
    {
        private const double MaxHandRPM = 140;
        private const double MaxUpDownBPM = 240;

        public HandMovements GenerateFromSongIntensityAndBeats(
            List<SongIntensity> songIntensities, 
            List<Beat> beats, 
            Difficulty difficulty,
            double skillLevel,
            int totalSamples, 
            int sampleRate, 
            double bpm)
        {
            var deltaT = 0.1;
            var resolution = (int)(deltaT * sampleRate);
            var continuousSongIntensity = new ContinuousLine2D(songIntensities.Select(x => new Point2D(x.SampleIndex, x.Intensity)));
            var handSpeedSkillMultiplier = DetermineHandSpeedMultiplier(difficulty, skillLevel);
            var nextBeatIdx = 0;

            var hands = (Hand[]) Enum.GetValues(typeof(Hand));
            var handPosition = hands.ToDictionary(hand => hand, _ => 0.0);
            var handDirection = hands.ToDictionary(hand => hand, _ => -1);
            var notes = hands.ToDictionary(hand => hand, hand => new List<Note>());
            var beatHand = Hand.Right;
            for (var sampleIdx = 0; sampleIdx < totalSamples; sampleIdx += resolution)
            {
                var isBeat = nextBeatIdx < beats.Count && sampleIdx > beats[nextBeatIdx].SampleIndex;

                var currentIntensity = continuousSongIntensity.ValueAtX(sampleIdx);
                if (currentIntensity.IsNaN())
                    currentIntensity = 0.0;
                var handSpeed = 0.1 * handSpeedSkillMultiplier * (MaxHandRPM / 60) * currentIntensity;
                if (isBeat)
                {
                    var beat = beats[nextBeatIdx];
                    if (IsBothHandBeat(beat))
                    {
                        handPosition[Hand.Right] += Math.PI;
                        handPosition[Hand.Left] += Math.PI;
                        var rightNote = BuildNote(Hand.Right, handPosition[Hand.Right], beat.SampleIndex, sampleRate, bpm);
                        var leftNote = BuildNote(Hand.Left, handPosition[Hand.Left], beat.SampleIndex, sampleRate, bpm);
                        notes[Hand.Right].Add(rightNote);
                        notes[Hand.Left].Add(leftNote);
                    }
                    else
                    {
                        if (ChangeBeatHand(currentIntensity))
                            beatHand = beatHand == Hand.Right ? Hand.Left : Hand.Right;
                        var note = BuildNote(beatHand, handPosition[beatHand], beat.SampleIndex, sampleRate, bpm);
                        notes[beatHand].Add(note);
                        handPosition[beatHand] += Math.PI;

                    }

                    if (ChangeHandDirection(currentIntensity))
                        handDirection[Hand.Right] *= -1;
                    if (ChangeHandDirection(currentIntensity))
                        handDirection[Hand.Left] *= -1;
                }
                else
                {
                    handPosition[Hand.Right] += handDirection[Hand.Right] * 2 * Math.PI * handSpeed * deltaT;
                    handPosition[Hand.Left] += handDirection[Hand.Left] * 2 * Math.PI * handSpeed * deltaT;
                }
                if (isBeat)
                    nextBeatIdx++;
            }

            return new HandMovements(notes[Hand.Left], notes[Hand.Right]);
        }

        private Note BuildNote(Hand hand, double handPosition, int sampleIndex, int sampleRate, double bpm)
        {
            var cutDirection = CutDirectionFromHandPosition(handPosition);
            var verticalPosition = cutDirection.InSet(CutDirection.Down, CutDirection.DownLeft, CutDirection.DownRight) ? VerticalPosition.Bottom
                : cutDirection.InSet(CutDirection.Up, CutDirection.UpLeft, CutDirection.UpRight) ? VerticalPosition.Middle
                : VerticalPosition.Top;
            var horizontalPosition = DetermineHorizontalPosition(cutDirection, hand);
            return new Note
            {
                Time = TimeConversion.SampleIndexToBeatIndex(sampleIndex, sampleRate, bpm),
                CutDirection = cutDirection,
                Hand = hand,
                HorizontalPosition = horizontalPosition,
                VerticalPosition = verticalPosition
            };
        }

        private HorizontalPosition DetermineHorizontalPosition(CutDirection cutDirection, Hand hand)
        {
            if (hand == Hand.Right)
            {
                var cutDirectionOptions = cutDirection.InSet(CutDirection.Left, CutDirection.UpLeft, CutDirection.DownLeft) ? new[] { HorizontalPosition.CenterLeft, HorizontalPosition.CenterRight}
                    : cutDirection.InSet(CutDirection.Right, CutDirection.UpRight, CutDirection.DownRight) ? new []{ HorizontalPosition.CenterRight, HorizontalPosition.Right}
                    : new []{ HorizontalPosition.CenterLeft, HorizontalPosition.CenterRight, HorizontalPosition.Right};
                return cutDirectionOptions[StaticRandom.Rng.Next(cutDirectionOptions.Length)];
            }
            else
            {
                var cutDirectionOptions = cutDirection.InSet(CutDirection.Left, CutDirection.UpLeft, CutDirection.DownLeft) ? new[] { HorizontalPosition.Left, HorizontalPosition.CenterLeft}
                    : cutDirection.InSet(CutDirection.Right, CutDirection.UpRight, CutDirection.DownRight) ? new []{ HorizontalPosition.CenterLeft, HorizontalPosition.CenterRight}
                    : new []{ HorizontalPosition.Left, HorizontalPosition.CenterLeft, HorizontalPosition.CenterRight};
                return cutDirectionOptions[StaticRandom.Rng.Next(cutDirectionOptions.Length)];
            }
        }

        private CutDirection CutDirectionFromHandPosition(double handPosition)
        {
            var angle = handPosition.Modulus(2 * Math.PI);
            if (angle.IsBetween(0, Math.PI / 8) || angle.IsBetween(15 * Math.PI / 8, 2 * Math.PI))
                return CutDirection.Left;
            if (angle.IsBetween(Math.PI / 8, 3 * Math.PI / 8))
                return CutDirection.DownLeft;
            if (angle.IsBetween(3 * Math.PI / 8, 5 * Math.PI / 8))
                return CutDirection.Down;
            if (angle.IsBetween(5 * Math.PI / 8, 7 * Math.PI / 8))
                return CutDirection.DownRight;
            if (angle.IsBetween(7 * Math.PI / 8, 9 * Math.PI / 8))
                return CutDirection.Right;
            if (angle.IsBetween(9 * Math.PI / 8, 11 * Math.PI / 8))
                return CutDirection.UpRight;
            if (angle.IsBetween(11 * Math.PI / 8, 13 * Math.PI / 8))
                return CutDirection.Up;
            if (angle.IsBetween(13 * Math.PI / 8, 15 * Math.PI / 8))
                return CutDirection.UpLeft;
            throw new Exception();
        }

        private static bool ChangeBeatHand(double currentIntensity)
        {
            return StaticRandom.Rng.NextDouble() < 0.5*currentIntensity;
        }

        private static bool IsBothHandBeat(Beat beat)
        {
            return beat.Strength > 0.3;
        }

        private static bool ChangeHandDirection(double songIntensity)
        {
            return StaticRandom.Rng.NextDouble() < songIntensity;
        }

        private double DetermineHandSpeedMultiplier(Difficulty difficulty, double skillLevel)
        {
            var skillLevelMultiplier = 0.5*(skillLevel+1);
            switch (difficulty)
            {
                case Difficulty.Easy:
                    return 0.5 * skillLevelMultiplier;
                case Difficulty.Normal:
                    return 0.8 * skillLevelMultiplier;
                case Difficulty.Hard:
                    return 0.9 * skillLevelMultiplier;
                case Difficulty.Expert:
                    return 1.0 * skillLevelMultiplier;
                default:
                    throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null);
            }
        }
    }

    public class HandMovements
    {
        public HandMovements(List<Note> leftHand, List<Note> rightHand)
        {
            LeftHand = leftHand;
            RightHand = rightHand;
        }

        public List<Note> LeftHand { get; }
        public List<Note> RightHand { get; }
    }
}
