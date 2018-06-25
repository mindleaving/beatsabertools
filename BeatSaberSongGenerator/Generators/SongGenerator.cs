using System;
using System.Linq;
using BeatSaberSongGenerator.AudioProcessing;
using BeatSaberSongGenerator.IO;
using BeatSaberSongGenerator.Objects;
using Commons;
using System.Collections.Generic;

namespace BeatSaberSongGenerator.Generators
{
    public class SongGenerator
    {
        private readonly BeatSaberSongGenerator.AudioProcessing.BeatDetector beatDetector;
        private readonly LevelInstructionGenerator levelInstructionGenerator;
            
        public SongGenerator(SongGeneratorSettings settings)
        {
            beatDetector = new BeatSaberSongGenerator.AudioProcessing.BeatDetector();
            levelInstructionGenerator = new LevelInstructionGenerator(settings);
        }

        public Song Generate(string songName, string author, string audioFilePath, string coverFilePath)
        {
            var audioMetadata = GetAudioMetadata(audioFilePath);
            audioMetadata.SongName = songName;
            audioMetadata.Author = author;
            var environmentType = SelectEnvironmentType();
            //var difficulties = new[] { Difficulty.Easy, Difficulty.Normal, Difficulty.Hard, Difficulty.Expert };
            var difficulties = new[] { Difficulty.Expert };
            var songInfo = new SongInfo
            {
                SongName = songName,
                SongSubName = "",
                AuthorName = author,
                BeatsPerMinute = (float) audioMetadata.BeatDetectorResult.BeatsPerMinute,
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

        private static EnvironmentType SelectEnvironmentType()
        {
            var environmentTypes = (EnvironmentType[]) Enum.GetValues(typeof(EnvironmentType));
            var environmentType = environmentTypes[StaticRandom.Rng.Next(environmentTypes.Length)];
            return environmentType;
        }

        private DifficultyLevel GenerateDifficultyLevel(Difficulty difficulty)
        {
            return new DifficultyLevel
            {
                AudioPath = SongStorer.SongPath,
                Difficulty = difficulty,
                InstructionPath = SongStorer.GenerateLevelFilePath(difficulty),
                Offset = 0,
                OldOffset = 0
            };
        }

        private AudioMetadata GetAudioMetadata(string audioFilePath)
        {
            var audioData = AudioSampleReader.ReadMonoSamples(audioFilePath, out var sampleRate);
            var songLength = TimeSpan.FromSeconds(audioData.Count / (double)sampleRate);
            var beatDetectorResult = beatDetector.DetectBeats(audioData, sampleRate);

            return new AudioMetadata
            {
                SampleRate = sampleRate,
                Length = songLength,
                BeatDetectorResult = beatDetectorResult
            };
        }
    }
}
