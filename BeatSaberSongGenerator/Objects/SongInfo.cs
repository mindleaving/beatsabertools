using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BeatSaberSongGenerator.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SongInfo
    {
        [JsonProperty("songName")]
        public string SongName { get; set; }

        [JsonProperty("songSubName")]
        public string SongSubName { get; set; }

        [JsonProperty("authorName")]
        public string AuthorName { get; set; }

        [JsonProperty("beatsPerMinute")]
        public float BeatsPerMinute { get; set; }

        [JsonProperty("previewStartTime")]
        public float PreviewStartTime { get; set; }

        [JsonProperty("previewDuration")]
        public float PreviewDuration { get; set; }

        [JsonProperty("coverImagePath")]
        public string CoverImagePath { get; set; }

        [JsonProperty("environmentName")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EnvironmentType EnvironmentName { get; set; }

        [JsonProperty("difficultyLevels")]
        public IList<DifficultyLevel> DifficultyLevels { get; set; }
    }

    public enum EnvironmentType
    {
        DefaultEnvironment = 0,
        TriangleEnvironment = 1,
        BigMirrorEnvironment = 2,
        NiceEnvironment = 3,
    }
}
