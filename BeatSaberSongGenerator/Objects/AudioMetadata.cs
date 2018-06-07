using System;
using System.Collections.Generic;
using BeatSaberSongGenerator.AudioProcessing;

namespace BeatSaberSongGenerator.Objects
{
    public class AudioMetadata
    {
        public string SongName { get; set; }
        public string Author { get; set; }
        public double BeatsPerMinute { get; set; }
        public int BeatsPerBar { get; set; }
        public int SampleRate { get; set; }
        public TimeSpan Length { get; set; }
        public List<Beat> Beats { get; set; }
        public List<SongIntensity> SongIntensities { get; set; }
    }
}