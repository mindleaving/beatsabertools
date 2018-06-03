using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BeatSaberSongGenerator.AudioProcessing;
using BeatSaberSongGenerator.IO;
using BeatSaberSongGenerator.Objects;
using NAudio.Wave;

namespace BeatSaberSongGenerator.Generators
{
    public class SongGenerator
    {
        private readonly LevelInstructionGenerator levelInstructionGenerator;

        public SongGenerator(SongGeneratorSettings settings)
        {
            levelInstructionGenerator = new LevelInstructionGenerator(settings);
        }

        public Song Generate(string songName, string author, string audioFilePath, string coverFilePath)
        {
            var audioMetadata = GetAudioMetadata(audioFilePath);
            audioMetadata.SongName = songName;
            audioMetadata.Author = author;
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
                AudioPath = SongStorer.SongPath,
                Difficulty = difficulty,
                InstructionPath = SongStorer.GenerateLevelFilePath(difficulty),
                Offset = 0,
                OldOffset = 0
            };
        }

        private AudioMetadata GetAudioMetadata(string audioFilePath)
        {
            //if(!Path.HasExtension(audioFilePath))
            //    throw new ArgumentException("Audio file must have an extension");
            //switch (Path.GetExtension(audioFilePath).ToLowerInvariant())
            //{
            //    case ".wav":
            //        break;
            //    default:
            //        throw new NotSupportedException("Audio file is in a format that is not supported");
            //}
            TimeSpan length;
            var audioData = AudioSampleReader.ReadMonoSamples(audioFilePath);
            using (var audioReader = new AudioFileReader(audioFilePath))
            {
                length = audioReader.TotalTime;
            }
            var bpm = DetermineBeatsPerMinute(audioData);
            return new AudioMetadata
            {
                Length = length,
                BeatsPerMinute = bpm,
                BeatsPerBar = 4
            };
        }

        private float DetermineBeatsPerMinute(IList<float> audioData)
        {
            File.WriteAllLines(@"C:\Temp\audiosamples.csv", audioData.Select(x => x.ToString("F3")));
            return 120;
        }
    }
}
