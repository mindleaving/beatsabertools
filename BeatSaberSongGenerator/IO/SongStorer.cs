using System.Collections.Generic;
using System.IO;
using BeatSaberSongGenerator.AudioProcessing;
using BeatSaberSongGenerator.Objects;
using Newtonsoft.Json;

namespace BeatSaberSongGenerator.IO
{
    public class SongStorer
    {
        public const string SongInfoFileName = "info.json";
        public const string SongPath = "song.ogg";
        public const string CoverImagePath = "cover.jpg";
        public static string GenerateLevelFilePath(Difficulty difficulty) => $"{difficulty.ToString().ToLowerInvariant()}.json";

        private readonly AudioToOggConverter audioToOggConverter = new AudioToOggConverter();

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

            if (!File.Exists(Path.Combine(outputDirectory, SongPath)))
            {
                if(Path.GetExtension(song.AudioPath).ToLowerInvariant() == ".ogg")
                    File.Copy(song.AudioPath, Path.Combine(outputDirectory, SongPath), overwrite: true);
                else
                    audioToOggConverter.Convert(song.AudioPath, Path.Combine(outputDirectory, SongPath));
            }
            File.Copy(song.CoverPath, Path.Combine(outputDirectory, CoverImagePath), overwrite: true);
        }

        private void StoreDifficultyLevel(
            LevelInstructions levelInstructions, 
            Difficulty difficulty, 
            string outputDirectory)
        {
            var instructionsJson = JsonConvert.SerializeObject(levelInstructions);
            File.WriteAllText(
                Path.Combine(outputDirectory, GenerateLevelFilePath(difficulty)),
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
            var audioPath = Path.Combine(directory, SongPath);
            var coverPath = Path.Combine(directory, CoverImagePath);
            return new Song(songInfo, difficultyLevels, audioPath, coverPath);
        }
    }
}
