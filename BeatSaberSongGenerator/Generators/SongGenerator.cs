using System.IO;
using System.Linq;
using BeatSaberSongGenerator.IO;
using BeatSaberSongGenerator.Objects;

namespace BeatSaberSongGenerator.Generators
{
    public class SongGenerator
    {
        private readonly LevelInstructionGenerator levelInstructionGenerator;

        public SongGenerator(SongGeneratorSettings settings)
        {
            levelInstructionGenerator = new LevelInstructionGenerator(settings);
        }

        public Song Generate(string audioFilePath, string coverFilePath)
        {
            var audioMetadata = GetAudioMetadata(audioFilePath);
            var songName = Path.GetFileNameWithoutExtension(audioFilePath);
            var author = audioMetadata.Author ?? string.Empty;
            var environmentType = EnvironmentType.DefaultEnvironment;
            var difficulties = new []{Difficulty.Normal};
            var songInfo = new SongInfo
            {
                SongName = songName,
                SongSubName = "",
                AuthorName = author,
                BeatsPerMinute = audioMetadata.BeatsPerMinute,
                PreviewStartTime = 0,
                PreviewDuration = 0,
                CoverImagePath = SongStorer.CoverImagePath,
                EnvironmentName = environmentType,
                DifficultyLevels = difficulties.Select(GenerateDifficultyLevel).ToList()
            };
            var levelInstructions = difficulties.ToDictionary(
                difficulty => difficulty, 
                difficulty => levelInstructionGenerator.Generate(difficulty, audioMetadata));
            return new Song(songInfo, levelInstructions, audioFilePath, coverFilePath);
        }

        private DifficultyLevel GenerateDifficultyLevel(Difficulty difficulty)
        {
            return new DifficultyLevel
            {
                AudioPath = SongStorer.SongInfoFileName,
                Difficulty = difficulty,
                InstructionPath = SongStorer.GenerateLevelFilePath(difficulty),
                Offset = 0,
                OldOffset = 0
            };
        }

        private AudioMetadata GetAudioMetadata(string audioFilePath)
        {
            throw new System.NotImplementedException();
        }
    }
}
