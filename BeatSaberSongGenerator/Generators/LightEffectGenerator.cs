using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberSongGenerator.Objects;
using Commons;
using Commons.Mathematics;
using MathNet.Numerics;

namespace BeatSaberSongGenerator.Generators
{
    public class LightEffectGenerator
    {
        public List<Event> Generate(AudioMetadata audioMetadata)
        {
            var redColorTheme = StaticRandom.Rng.Next(2) == 0;
            var blueColorTheme = !redColorTheme;

            var lightEffects = new List<Event>();
            var bpm = audioMetadata.BeatDetectorResult.BeatsPerMinute;
            var songIntensity = audioMetadata.BeatDetectorResult.SongIntensities;
            var continuousSongIntensity = new ContinuousLine2D(songIntensity.Select(x => new Point2D(x.SampleIndex, x.Intensity)));
            for (var beatIdx = 0; beatIdx < audioMetadata.BeatDetectorResult.RegularBeats.Count; beatIdx++)
            {
                if(beatIdx.IsOdd())
                    continue; // Skip every second beat
                var regularBeat = audioMetadata.BeatDetectorResult.RegularBeats[beatIdx];
                var time = TimeConversion.SampleIndexToRealTime(regularBeat.SampleIndex, audioMetadata.SampleRate);
                var timeAsBeatIndex = TimeConversion.SampleIndexToBeatIndex(regularBeat.SampleIndex, audioMetadata.SampleRate, bpm);
                if (time > audioMetadata.Length - TimeSpan.FromSeconds(3))
                {
                    lightEffects.AddRange(TurnOffAllLights(timeAsBeatIndex));
                    break;
                }

                var currentIntensity = continuousSongIntensity.ValueAtX(regularBeat.SampleIndex);
                if (currentIntensity < 0.5)
                {
                    var tunnelMove = StaticRandom.Rng.Next(2) == 0 ? EventType.TunnelRotation : EventType.TunnelZooming;
                    lightEffects.Add(new Event
                    {
                        Time = timeAsBeatIndex,
                        Type = tunnelMove,
                        Value = StaticRandom.Rng.Next(10)
                    });
                }
                else
                {
                    lightEffects.Add(new Event
                    {
                        Time = timeAsBeatIndex,
                        Type = EventType.LightEffect0,
                        Value = (int) (redColorTheme ? LightColor.RedFadeOut : LightColor.BlueFadeOut)
                    });
                    lightEffects.Add(new Event
                    {
                        Time = timeAsBeatIndex,
                        Type = EventType.LightEffect1,
                        Value = (int) (redColorTheme ? LightColor.RedFadeOut : LightColor.BlueFadeOut)
                    });
                }
            }

            return lightEffects;
        }

        private static IEnumerable<Event> TurnOffAllLights(float time)
        {
            var allEvents = (EventType[])Enum.GetValues(typeof(EventType));
            return allEvents.Select(eventType => new Event
            {
                Time = time,
                Type = eventType,
                Value = 0
            });
        }
    }
}
