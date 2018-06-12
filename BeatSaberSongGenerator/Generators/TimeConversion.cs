using System;

namespace BeatSaberSongGenerator.Generators
{
    public static class TimeConversion
    {
        public static float SampleIndexToBeatIndex(int sampleIndex, int sampleRate, double bpm)
        {
            var beatsPerSecond = bpm / 60;
            return (float) (sampleIndex * beatsPerSecond / sampleRate);
        }

        public static TimeSpan SampleIndexToRealTime(int sampleIndex, int sampleRate)
        {
            return TimeSpan.FromSeconds(sampleIndex / (double)sampleRate);
        }

        public static TimeSpan BeatIndexToRealTime(float beatIndex, double bpm)
        {
            return TimeSpan.FromSeconds(beatIndex * 60 / bpm);
        }

        public static int BeatIndexToSampleIndex(float beatIndex, double bpm, int sampleRate)
        {
            return (int) (beatIndex * 60 * sampleRate / bpm);
        }
    }
}
