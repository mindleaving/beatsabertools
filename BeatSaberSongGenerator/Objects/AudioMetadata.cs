using System;

namespace BeatSaberSongGenerator.Objects
{
    public class AudioMetadata
    {
        public string SongName { get; set; }
        public string Author { get; set; }
        public float BeatsPerMinute { get; set; }
        public int BeatsPerBar { get; set; }
        public TimeSpan Length { get; set; }
    }
}