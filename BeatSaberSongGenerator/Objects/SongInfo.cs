using System.Collections.Generic;
using Newtonsoft.Json;

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
        public string EnvironmentName { get; set; }

        [JsonProperty("difficultyLevels")]
        public IList<DifficultyLevel> DifficultyLevels { get; set; }
    }
}
