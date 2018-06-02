﻿using System.Collections.Generic;

namespace BeatSaberLevelGenerator.Objects
{
    public class Song
    {
        public Song(
            SongInfo songInfo, 
            Dictionary<Difficulty, LevelInstructions> difficultyLevels)
        {
            SongInfo = songInfo;
            DifficultyLevels = difficultyLevels;
        }

        public SongInfo SongInfo { get; }
        public Dictionary<Difficulty, LevelInstructions> DifficultyLevels { get; }
    }
}