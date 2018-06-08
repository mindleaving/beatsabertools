using System.Collections.Generic;
using System.Linq;
using Commons.DataProcessing;
using Commons.Extensions;

namespace BeatSaberSongGenerator.AudioProcessing
{
    public static class BeatMerger
    {
        public static List<Beat> Merge(List<Beat> detectedBeats, List<Beat> regularBeats, int sampleRate)
        {
            var mergedBeats = new List<Beat>();
            var beatTolerance = 0.05 * sampleRate;
            var detectedBeatWindow = new SlidingWindow<Beat>(detectedBeats, beat => beat.SampleIndex, beatTolerance, WindowPositioningType.CenteredAtPosition);
            foreach (var regularBeat in regularBeats)
            {
                detectedBeatWindow.SetPosition(regularBeat.SampleIndex);
                if(detectedBeatWindow.Any())
                {
                    var matchingDetectedBeat = detectedBeatWindow.MaximumItem(beat => beat.Strength);
                    mergedBeats.Add(matchingDetectedBeat);
                }
                else
                {
                    mergedBeats.Add(regularBeat);
                }
            }
            return mergedBeats;
        }
    }
}
