using System;
using System.Collections.Generic;
using BeatSaberSongGenerator.Objects;
using Commons;
using Commons.Mathematics;

namespace BeatSaberSongGenerator.Generators
{
    public class LevelInstructionGenerator
    {
        private readonly SongGeneratorSettings settings;

        public LevelInstructionGenerator(SongGeneratorSettings settings)
        {
            this.settings = settings;
        }

        public LevelInstructions Generate(Difficulty difficulty, AudioMetadata audioMetadata)
        {
            var events = new List<Event>();
            //var notes = new List<Note>();
            //var obstacles = new List<Obstacle>();
            var notes = GenerateRandomNotes(difficulty, audioMetadata);
            var obstacles = GenerateRandomObstacle(difficulty, audioMetadata);
            return new LevelInstructions
            {
                Version = "1.5.0",
                BeatsPerMinute = audioMetadata.BeatsPerMinute,
                BeatsPerBar = audioMetadata.BeatsPerBar,
                NoteJumpSpeed = 10,
                Shuffle = 0,
                ShufflePeriod = 0.5f,
                Events = events,
                Notes = notes,
                Obstacles = obstacles
            };
        }

        private IList<Note> GenerateRandomNotes(Difficulty difficulty, AudioMetadata audioMetadata)
        {
            var notes = new List<Note>();
            var nextHand = Hand.Right;
            var stepSize = difficulty == Difficulty.Easy ? 4.0
                : difficulty == Difficulty.Normal ? 2.0
                : difficulty == Difficulty.Hard ? 1.0
                : difficulty == Difficulty.Expert ? 0.5
                : throw new ArgumentOutOfRangeException(nameof(difficulty));
            var beatsPerSecond = audioMetadata.BeatsPerMinute / 60;
            var totalBeats = audioMetadata.Length.TotalSeconds * beatsPerSecond;
            foreach (var beatIdx in SequenceGeneration.FixedStep(5, totalBeats, stepSize))
            {
                var cutDirection = (CutDirection) StaticRandom.Rng.Next(0, 9);
                var note = new Note
                {
                    Time = (float) beatIdx,
                    CutDirection = cutDirection,
                    Hand = nextHand,
                    HorizontalPosition = nextHand == Hand.Right ? HorizontalPosition.Right : HorizontalPosition.Left,
                    VerticalPosition = VerticalPosition.Bottom
                };
                notes.Add(note);

                nextHand = nextHand == Hand.Right ? Hand.Left : Hand.Right;
            }
            return notes;
        }

        private IList<Obstacle> GenerateRandomObstacle(Difficulty difficulty, AudioMetadata audioMetadata)
        {
            var obstacles = new List<Obstacle>();
            var beatsPerSecond = audioMetadata.BeatsPerMinute / 60;
            var totalBeats = audioMetadata.Length.TotalSeconds * beatsPerSecond;
            foreach (var beatIdx in SequenceGeneration.FixedStep(10, totalBeats, 10))
            {
                var obstacle = new Obstacle
                {
                    Time = (float)beatIdx,
                    Duration = 5,
                    Type = ObstableType.Wall,
                    Width = 1,
                    HorizontalPosition = (HorizontalPosition)StaticRandom.Rng.Next(0, 4)
                };
                obstacles.Add(obstacle);
            }
            return obstacles;
        }
    }
}