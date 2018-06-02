using System.Collections.Generic;
using System.IO;
using BeatSaberSongGenerator.Objects;
using Newtonsoft.Json;

namespace BeatSaberSongGenerator.IO
{
    public class SongStorer
    {
        public const string SongInfoFileName = "info.json";

        public void Store(Song song, string outputDirectory)
        {
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);
            var songInfoJson = JsonConvert.SerializeObject(song.SongInfo);
            File.WriteAllText(
                Path.Combine(outputDirectory, SongInfoFileName),
                songInfoJson);
            foreach (var difficultyLevel in song.DifficultyLevels)
            {
                StoreDifficultyLevel(difficultyLevel.Value, difficultyLevel.Key, outputDirectory);
            }
        }

        private void StoreDifficultyLevel(
            LevelInstructions levelInstructions, 
            Difficulty difficulty, 
            string outputDirectory)
        {
            var instructionsJson = JsonConvert.SerializeObject(levelInstructions);
            File.WriteAllText(
                Path.Combine(outputDirectory, $"{difficulty.ToString().ToLowerInvariant()}.json"),
                instructionsJson);
        }

        public Song Load(string directory)
        {
            var songInfoJson = File.ReadAllText(Path.Combine(directory, SongInfoFileName));
            var songInfo = JsonConvert.DeserializeObject<SongInfo>(songInfoJson);
            var difficultyLevels = new Dictionary<Difficulty, LevelInstructions>();
            foreach (var difficultyLevel in songInfo.DifficultyLevels)
            {
                var instructionPath = Path.Combine(directory, difficultyLevel.InstructionPath);
                var instructionJson = File.ReadAllText(instructionPath);
                var levelInstructions = JsonConvert.DeserializeObject<LevelInstructions>(instructionJson);
                difficultyLevels.Add(difficultyLevel.Difficulty, levelInstructions);
            }
            return new Song(songInfo, difficultyLevels);
        }
    }
}
