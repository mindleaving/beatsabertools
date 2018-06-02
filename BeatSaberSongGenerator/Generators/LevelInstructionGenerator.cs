using System.Collections.Generic;
using BeatSaberSongGenerator.Objects;

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
            var notes = new List<Note>();
            var obstacles = new List<Obstacle>();
            return new LevelInstructions
            {
                Version = "1.0.0",
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
    }
}