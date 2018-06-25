using System.Collections.Generic;
using NAudio.Wave;

namespace BeatSaberSongGenerator.AudioProcessing
{
    public static class AudioSampleReader
    {
        public static IList<float> ReadMonoSamples(string audioFilePath, out int sampleRate, out double songLength)
        {
            var audioData = new List<float>();
            var buffer = new float[1];
            using (var audioReader = new AudioFileReader(audioFilePath))
            {
                songLength = audioReader.TotalTime.TotalSeconds;
                var sampleProvider = audioReader.ToSampleProvider();
                sampleRate = sampleProvider.WaveFormat.SampleRate;
                var channelCount = sampleProvider.WaveFormat.Channels;
                if (channelCount > 1)
                    sampleProvider = sampleProvider.ToMono();
                var offset = 0;
                while (sampleProvider.Read(buffer, offset, 1) > 0)
                {
                    audioData.Add(buffer[0]);
                }
            }
            return audioData;
        }

        public static List<IList<float>> ReadChannels(string audioFilePath, out WaveFormat waveFormat)
        {
            var audioData = new List<IList<float>>();
            var buffer = new float[1];
            using (var audioReader = new AudioFileReader(audioFilePath))
            {
                var sampleProvider = audioReader.ToSampleProvider();
                waveFormat = sampleProvider.WaveFormat;
                var channelCount = sampleProvider.WaveFormat.Channels;
                for (int channelIdx = 0; channelIdx < channelCount; channelIdx++)
                {
                    audioData.Add(new List<float>());
                }
                var offset = 0;
                var channel = 0;
                while (sampleProvider.Read(buffer, offset, 1) > 0)
                {
                    audioData[channel].Add(buffer[0]);
                    channel++;
                    if (channel >= channelCount)
                        channel = 0;
                }
            }

            return audioData;
        }
    }
}