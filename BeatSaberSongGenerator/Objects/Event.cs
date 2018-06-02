using Newtonsoft.Json;

namespace BeatSaberSongGenerator.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Event
    {
        [JsonProperty("_time")]
        public float Time { get; set; }

        [JsonProperty("_type")]
        public EventType Type { get; set; }

        [JsonProperty("_value")]
        public int Value { get; set; }
    }
}