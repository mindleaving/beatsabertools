using System.Collections.Generic;

namespace BeatSaberSongGenerator.AudioProcessing
{
    public static class AudioDownsampler
    {
        public static IList<float> Downsample(IList<float> audioData, int downsampling)
        {
            var downsampledData = new List<float>();
            var startIdx = 0;
            while (audioData.Count > startIdx+downsampling)
            {
                var sample = 0.0f;
                for (int sampleIdx = startIdx; sampleIdx < startIdx+downsampling; sampleIdx++)
                {
                    sample += audioData[sampleIdx];
                }
                sample /= downsampling;
                downsampledData.Add(sample);
                startIdx += downsampling;
            }
            return downsampledData;
        }
    }
}
