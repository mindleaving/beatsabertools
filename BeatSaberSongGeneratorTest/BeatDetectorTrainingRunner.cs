using System;
using System.Collections.Generic;
using BeatSaberSongGenerator.AudioProcessing;
using NUnit.Framework;

namespace BeatSabeSonglGeneratorTest
{
    [TestFixture]
    public class BeatDetectorTrainingRunner
    {
        [Test]
        public void ApplyModel()
        {
            var songFilePath = @"C:\Users\Jan\Music\Dimitri Vegas - Ocarina.mp3";
            var sut = new BeatDetector();
            var signal = AudioSampleReader.ReadMonoSamples(songFilePath, out var sampleRate, out var songLength);
            var beatDetectorResult = sut.DetectBeats(signal, sampleRate, songLength);
            Console.WriteLine($@"BPM: {beatDetectorResult.BeatsPerMinute:F0}");
            foreach (var beat in beatDetectorResult.DetectedBeats)
            {
                Console.WriteLine($@"{beat.SampleIndex};{beat.Strength}");
            }
        }
    }
}
