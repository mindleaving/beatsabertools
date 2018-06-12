using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberSongGenerator.AudioProcessing;
using BeatSaberSongGenerator.Objects;
using Commons;
using Commons.Extensions;
using Commons.Mathematics;

namespace BeatSaberSongGenerator.Generators
{
    public class LevelInstructionGenerator
    {
        private readonly SongGeneratorSettings settings;
        private readonly LightEffectGenerator lightEffectGenerator;
        private readonly HandMovementGenerator handMovementGenerator;
        private readonly BaseRhythmGenerator baseRhythmGenerator;
        private readonly ObstacleGenerator obstacleGenerator;

        public LevelInstructionGenerator(SongGeneratorSettings settings)
        {
            this.settings = settings;
            lightEffectGenerator = new LightEffectGenerator();
            handMovementGenerator = new HandMovementGenerator();
            baseRhythmGenerator = new BaseRhythmGenerator();
            obstacleGenerator = new ObstacleGenerator();
        }

        public LevelInstructions Generate(Difficulty difficulty, AudioMetadata audioMetadata)
        {
            var events = lightEffectGenerator.Generate(audioMetadata);
            //var notes = GenerateNotesFromSongAnalysis(difficulty, audioMetadata);
            var notes = GenerateModifiedBaseRhythm(difficulty, audioMetadata);
            var obstacles = obstacleGenerator.Generate(difficulty, audioMetadata);
            return new LevelInstructions
            {
                Version = "1.5.0",
                BeatsPerMinute = (float) audioMetadata.BeatDetectorResult.BeatsPerMinute,
                BeatsPerBar = audioMetadata.BeatDetectorResult.BeatsPerBar,
                NoteJumpSpeed = 10,
                Shuffle = 0,
                ShufflePeriod = 0.5f,
                Events = events,
                Notes = notes,
                Obstacles = obstacles
            };
        }

        private IList<Note> GenerateModifiedBaseRhythm(Difficulty difficulty, AudioMetadata audioMetadata)
        {
            var beats = BeatMerger.Merge(
                audioMetadata.BeatDetectorResult.DetectedBeats, 
                audioMetadata.BeatDetectorResult.RegularBeats, 
                audioMetadata.SampleRate);
            var startBeatIdx = beats.FindIndex(beat => beat.Strength > 0);
            var endBeatIdx = beats.FindLastIndex(beat => beat.Strength > 0);
            var totalValidBeatCount = endBeatIdx - startBeatIdx + 1;
            var barCount = totalValidBeatCount / audioMetadata.BeatDetectorResult.BeatsPerBar;
            var notes = baseRhythmGenerator.Generate(audioMetadata.BeatDetectorResult.BeatsPerBar, barCount, startBeatIdx);
            var difficultyFilteredNotes = FilterNotesByDifficulty(notes, audioMetadata, difficulty);
            return difficultyFilteredNotes;
        }

        private IList<Note> GenerateNotesFromSongAnalysis(Difficulty difficulty, AudioMetadata audioMetadata)
        {
            var sampleRate = audioMetadata.SampleRate;
            var bpm = audioMetadata.BeatDetectorResult.BeatsPerMinute;
            var totalSamples = (int)(audioMetadata.Length.TotalSeconds * sampleRate);
            var songIntensities = audioMetadata.BeatDetectorResult.SongIntensities;
            var beats = BeatMerger.Merge(
                audioMetadata.BeatDetectorResult.DetectedBeats, 
                audioMetadata.BeatDetectorResult.RegularBeats, 
                sampleRate);
            var handMovements = handMovementGenerator.GenerateFromSongIntensityAndBeats(
                songIntensities, 
                beats, 
                difficulty, 
                settings.SkillLevel, 
                totalSamples, 
                sampleRate, 
                bpm);
            var baseNotes = handMovements.LeftHand.Concat(handMovements.RightHand).OrderBy(x => x.Time);

            var notes = FilterNotesByDifficulty(baseNotes, audioMetadata, difficulty);
            return notes;
        }

        private List<Note> FilterNotesByDifficulty(IEnumerable<Note> baseNotes, AudioMetadata audioMetadata, Difficulty difficulty)
        {
            var songIntensities = audioMetadata.BeatDetectorResult.SongIntensities;
            var bpm = audioMetadata.BeatDetectorResult.BeatsPerMinute;
            var sampleRate = audioMetadata.SampleRate;
            var continuousSongIntensity = new ContinuousLine2D(songIntensities.Select(x => new Point2D(x.SampleIndex, x.Intensity)));
            var minimumTimeBetweenNotes = DetermineTimeBetweenNotes(difficulty);
            var notes = new List<Note>();
            Note lastNote = null;
            foreach (var baseNote in baseNotes)
            {
                var noteTime = TimeConversion.BeatIndexToRealTime(baseNote.Time, bpm);
                var noteTimeInSamples = noteTime.TotalSeconds * sampleRate;
                if (noteTime < TimeSpan.FromSeconds(3))
                    continue; // Stop a few seconds before song ends
                if (noteTime > audioMetadata.Length - TimeSpan.FromSeconds(3))
                    break; // Stop a few seconds before song ends
                var currentIntensity = continuousSongIntensity.ValueAtX(noteTimeInSamples);
                if (lastNote != null)
                {
                    var timeSinceLastBeat = noteTime
                                            - TimeConversion.BeatIndexToRealTime(lastNote.Time, bpm);
                    if (currentIntensity.IsNaN())
                        currentIntensity = 0;
                    var intensityAdjustment = TimeSpan.FromSeconds(0.5 * (1 - currentIntensity));
                    if (timeSinceLastBeat < minimumTimeBetweenNotes + intensityAdjustment
                        && !AreSimultaneous(baseNote, lastNote))
                    {
                        continue;
                    }
                }
                var noteProbability = currentIntensity < 0.3 ? 0 : currentIntensity;
                if(StaticRandom.Rng.NextDouble() > noteProbability)
                    continue;
                notes.Add(baseNote);
                lastNote = baseNote;
            }
            return notes;
        }

        private static bool AreSimultaneous(Note baseNote, Note lastNote)
        {
            return (baseNote.Time - (double)lastNote.Time).Abs() < 0.1;
        }

        private TimeSpan DetermineTimeBetweenNotes(Difficulty difficulty)
        {
            double multiplier;
            switch (difficulty)
            {
                case Difficulty.Easy:
                    multiplier = 1.5;
                    break;
                case Difficulty.Normal:
                    multiplier = 1.0;
                    break;
                case Difficulty.Hard:
                    multiplier = 0.5;
                    break;
                case Difficulty.Expert:
                    multiplier = 0.3;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null);
            }
            return TimeSpan.FromSeconds(multiplier * (1-settings.SkillLevel));
        }
    }
}