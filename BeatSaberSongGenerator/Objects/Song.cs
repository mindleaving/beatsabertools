using System.Collections.Generic;

namespace BeatSaberSongGenerator.Objects
{
    public class Song
    {
        public Song(
            SongInfo songInfo, 
            Dictionary<Difficulty, LevelInstructions> difficultyLevels, 
            string audioPath, 
            string coverPath)
        {
            SongInfo = songInfo;
            DifficultyLevels = difficultyLevels;
            AudioPath = audioPath;
            CoverPath = coverPath;
        }

        public SongInfo SongInfo { get; }
        public Dictionary<Difficulty, LevelInstructions> DifficultyLevels { get; }
        public string AudioPath { get; }
        public string CoverPath { get; }
    }
}
