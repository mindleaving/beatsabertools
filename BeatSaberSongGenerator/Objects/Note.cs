using Newtonsoft.Json;

namespace BeatSaberSongGenerator.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Note
    {
        public Note() {}

        public Note(
            int beatIndex,
            Hand hand, 
            CutDirection cutDirection, 
            HorizontalPosition horizontalPosition,
            VerticalPosition verticalPosition)
        {
            BeatIndex = beatIndex;
            Time = 0;
            Hand = hand;
            CutDirection = cutDirection;
            HorizontalPosition = horizontalPosition;
            VerticalPosition = verticalPosition;
        }

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


        public int BeatIndex { get; set; }
    }
}