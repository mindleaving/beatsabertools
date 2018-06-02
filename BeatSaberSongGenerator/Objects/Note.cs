using Newtonsoft.Json;

namespace BeatSaberSongGenerator.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Note
    {
        [JsonProperty("_time")]
        public float Time { get; set; }

        [JsonProperty("_lineIndex")]
        public HorizontalPosition HorizontalPosition { get; set; }

        [JsonProperty("_lineLayer")]
        public VerticalPosition VerticalPosition { get; set; }

        [JsonProperty("_type")]
        public Hand Hand { get; set; }

        [JsonProperty("_cutDirection")]
        public CutDirection CutDirection { get; set; }
    }
}