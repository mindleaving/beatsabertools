namespace BeatSaberSongGenerator.Objects
{
    public class SongIntensity
    {
        public SongIntensity(int sampleIndex, double intensity)
        {
            SampleIndex = sampleIndex;
            Intensity = intensity;
        }

        public int SampleIndex { get; }
        public double Intensity { get; }
    }
}
