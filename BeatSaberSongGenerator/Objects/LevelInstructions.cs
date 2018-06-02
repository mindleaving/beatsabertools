using System.Collections.Generic;
using Newtonsoft.Json;

namespace BeatSaberSongGenerator.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LevelInstructions
    {
        [JsonProperty("_version")]
        public string Version { get; set; }

        [JsonProperty("_beatsPerMinute")]
        public int BeatsPerMinute { get; set; }

        [JsonProperty("_beatsPerBar")]
        public int BeatsPerBar { get; set; }

        [JsonProperty("_noteJumpSpeed")]
        public int NoteJumpSpeed { get; set; }

        [JsonProperty("_shuffle")]
        public int Shuffle { get; set; }

        [JsonProperty("_shufflePeriod")]
        public float ShufflePeriod { get; set; }

        [JsonProperty("_events")]
        public IList<Event> Events { get; set; }

        [JsonProperty("_notes")]
        public IList<Note> Notes { get; set; }

        [JsonProperty("_obstacles")]
        public IList<Obstacle> Obstacles { get; set; }
    }
}