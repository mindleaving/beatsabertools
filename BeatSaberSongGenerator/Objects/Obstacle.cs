using Newtonsoft.Json;

namespace BeatSaberSongGenerator.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Obstacle
    {
        [JsonProperty("_time")]
        public float Time { get; set; }

        [JsonProperty("_lineIndex")]
        public HorizontalPosition HorizontalPosition { get; set; }

        [JsonProperty("_type")]
        public ObstableType Type { get; set; }

        [JsonProperty("_duration")]
        public float Duration { get; set; }

        [JsonProperty("_width")]
        public int Width { get; set; }
    }
}