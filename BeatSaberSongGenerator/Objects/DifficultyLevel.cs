using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BeatSaberSongGenerator.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DifficultyLevel
    {
        [JsonProperty("difficulty")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Difficulty Difficulty { get; set; }

        [JsonProperty("difficultyRank")]
        public int DifficultyRank => (int)Difficulty;

        [JsonProperty("audioPath")]
        public string AudioPath { get; set; }

        [JsonProperty("jsonPath")]
        public string InstructionPath { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("oldOffset")]
        public int OldOffset { get; set; }
    }
}