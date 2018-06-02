using System.Collections.Generic;
using BeatSaberSongGenerator.IO;
using BeatSaberSongGenerator.Objects;
using NUnit.Framework;

namespace BeatSabeSonglGeneratorTest
{
    [TestFixture]
    public class SongSerializerTest
    {
        [Test]
        public void SongInfoSerializationRoundTrip()
        {
            var songName = "Test Song";
            var song = CreateSong(songName);

            var sut = new SongStorer();
            var directory = @"C:\Temp\BeatSaberSongGeneratorTest";
            sut.Store(song, directory);
            var deserializedSong = sut.Load(directory);

            Assert.That(deserializedSong.SongInfo.SongName, Is.EqualTo(songName));
            Assert.That(deserializedSong.SongInfo.DifficultyLevels.Count, Is.EqualTo(1));
        }

        private static Song CreateSong(string songName)
        {
            var difficulty = Difficulty.Normal;
            var songInfo = new SongInfo
            {
                SongName = songName,
                AuthorName = "Jan",
                DifficultyLevels = new List<DifficultyLevel>
                {
                    new DifficultyLevel
                    {
                        AudioPath = SongStorer.SongPath,
                        Difficulty = difficulty,
                        InstructionPath = SongStorer.GenerateLevelFilePath(difficulty),
                        Offset = 0,
                        OldOffset = 0
                    }
                }
            };
            var difficultyLevels = new Dictionary<Difficulty, LevelInstructions>
            {
                {
                    difficulty, new LevelInstructions
                    {
                        Version = "1.0.0",
                        BeatsPerBar = 4,
                        BeatsPerMinute = 120,
                        NoteJumpSpeed = 10,
                        Shuffle = 1,
                        ShufflePeriod = 0.5f,
                        Events = new List<Event>
                            {new Event {Time = 3.4f, Type = EventType.MoveLight2, Value = 1}},
                        Notes = new List<Note>
                        {
                            new Note
                            {
                                Time = 2.2f,
                                VerticalPosition = VerticalPosition.Middle,
                                HorizontalPosition = HorizontalPosition.Center,
                                Hand = Hand.Left,
                                CutDirection = CutDirection.Any
                            }
                        },
                        Obstacles = new List<Obstacle>
                        {
                            new Obstacle
                            {
                                Time = 4.2f,
                                Type = ObstableType.Wall,
                                HorizontalPosition = HorizontalPosition.CenterLeft,
                                Duration = 3.5f,
                                Width = 1
                            }
                        }
                    }
                }
            };
            var song = new Song(songInfo, difficultyLevels, SongStorer.SongPath, SongStorer.CoverImagePath);
            return song;
        }
    }
}
