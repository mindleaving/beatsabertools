using System;
using System.Collections.Generic;
using BeatSaberSongGenerator.AudioProcessing;
using NUnit.Framework;

namespace BeatSabeSonglGeneratorTest
{
    [TestFixture]
    public class BeatDetectorTrainingRunner
    {
        private readonly string modelOutputFile = @"C:\Temp\beatDetectorModel.bin";

        [Test]
        public void RunTraining()
        {
            var sut = new BeatDetectorTraining();
            var songFileToBeatPositionHeaderMap = new Dictionary<string, string>
            {
                { @"C:\Users\Jan\Music\Dimitri Vegas - Ocarina.mp3", "Ocarina" },
                //{ @"C:\Users\Jan\Music\Red Hot Chili Peppers - Californication.mp3", "Californication" },
                //{ @"C:\Users\Jan\Music\Dr. Alban - Its My Life.mp3", "It’s my life" },
                //{ @"C:\Users\Jan\Music\P!nk - Try.mp3", "Try" },
                //{ @"C:\Users\Jan\Music\Juli - Perfekte Welle.mp3", "Perfekte Welle" },
                //{ @"C:\Users\Jan\Music\Taylor Swift - Safe  Sound.mp3", "Safe and Sound" },
                //{ @"C:\Users\Jan\Music\Europe - The Final Countdown.mp3", "Finale Countdown" },
                //{ @"C:\Users\Jan\Music\Survivor - Eye Of The Tiger.mp3", "Eye of the tiger" },
                //{ @"C:\Users\Jan\Music\Pur - Prinzessin  Funkelperlenaugen.mp3", "Prinzessin/Funkelperlenaugen" },
                //{ @"C:\Users\Jan\Music\Arctic Monkeys - Mardy Bum.mp3", "Mardy Bum" },
                //{ @"C:\Users\Jan\Music\Clarity - Zedd.mp3", "Clarity" },
            };
            var beatPositionFilePath = @"C:\Users\Jan\Music\beatPositions.csv";
            sut.Train(songFileToBeatPositionHeaderMap, beatPositionFilePath, modelOutputFile);
        }

        [Test]
        public void ApplyModel()
        {
            var songFilePath = @"C:\Users\Jan\Music\Dimitri Vegas - Ocarina.mp3";
            var sut = new BeatDetector();
            var signal = AudioSampleReader.ReadMonoSamples(songFilePath, out var sampleRate);
            var beatDetectorResult = sut.DetectBeats(signal, sampleRate);
            Console.WriteLine($@"BPM: {beatDetectorResult.BeatsPerMinute:F0}");
            foreach (var beat in beatDetectorResult.Beats)
            {
                Console.WriteLine($@"{beat.SampleIndex};{beat.Strength}");
            }
        }
    }
}
