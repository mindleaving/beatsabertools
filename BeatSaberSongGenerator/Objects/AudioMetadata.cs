using System;
using System.Collections.Generic;
using BeatSaberSongGenerator.AudioProcessing;

namespace BeatSaberSongGenerator.Objects
{
    public class AudioMetadata
    {
        public string SongName { get; set; }
        public string Author { get; set; }
        public int SampleRate { get; set; }
        public TimeSpan Length { get; set; }
        public BeatDetectorResult BeatDetectorResult { get; set; }
    }
}