using System;
using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Commons.Mathematics;
using NWaves.Transforms;
using NWaves.Windows;

namespace BeatSaberSongGenerator.AudioProcessing
{
    public class BeatDetector
    {
        /// <summary>
        /// Takes an audio signal, detects the beats and returns a list of all times (in seconds) where a beat is located
        /// </summary>
        /// <param name="signal">Expected to be normalized to -1 to 1</param>
        /// <param name="sampleRate">Sample rate of signal</param>
        /// <returns>Sample indices of beats</returns>
        public List<int> DetectBeats(IList<float> signal, int sampleRate)
        {
            var stftWindowSize = 4096;
            var stepSize = 1024;
            var stft = new Stft(windowSize: stftWindowSize, hopSize: stepSize, window: WindowTypes.Hamming);
            var spectrogram = stft.Spectrogram(signal.ToArray());
            var windowPositions = SequenceGeneration.Linspace(stftWindowSize / 2, signal.Count-stftWindowSize/2, spectrogram.Count)
                .ToList();

            var fftMagnitudeIncreaseSeries = new List<Point2D>();
            for (int stepIdx = 1; stepIdx < spectrogram.Count; stepIdx++)
            {
                var windowCenterSample = windowPositions[stepIdx];
                var previousFft = spectrogram[stepIdx - 1];
                var currentFft = spectrogram[stepIdx];

                var magnitudeIncreaseCount = 0;
                for (int frequencyBinIdx = 0; frequencyBinIdx < currentFft.Length; frequencyBinIdx++)
                {
                    var isIncrease = currentFft[frequencyBinIdx] > previousFft[frequencyBinIdx];
                    if (isIncrease)
                        magnitudeIncreaseCount++;
                }
                fftMagnitudeIncreaseSeries.Add(new Point2D(windowCenterSample, magnitudeIncreaseCount));
            }

            var thresholdWindowSize = (int)Math.Floor(1.5 * sampleRate / stepSize); // Corresponds to 1.5 seconds
            var dynamicThreshold = ComputeDynamicThreshold(
                fftMagnitudeIncreaseSeries.Select(p => p.Y).ToList(), 
                thresholdWindowSize, 2, 4);
            var candidateBeatPositions = new List<int>();
            for (var pointIdx = 0; pointIdx < fftMagnitudeIncreaseSeries.Count; pointIdx++)
            {
                var timePoint = fftMagnitudeIncreaseSeries[pointIdx];
                var threshold = dynamicThreshold[pointIdx];
                if(timePoint.Y > threshold)
                    candidateBeatPositions.Add((int)timePoint.X);
            }

            var lowerLimit = 0.1 * sampleRate;
            var upperLimit = 1.0 * sampleRate;
            var filteredBeats = new List<int>();
            var lastBeatPosition = 0;
            foreach (var candidateBeatPosition in candidateBeatPositions)
            {
                if(candidateBeatPosition - lastBeatPosition < lowerLimit)
                    continue;
                //if(candidateBeatPosition - lastBeatPosition > upperLimit)
                //    filteredBeats.Add((candidateBeatPosition + lastBeatPosition) / 2);
                filteredBeats.Add(candidateBeatPosition);
                lastBeatPosition = candidateBeatPosition;
            }
            // TODO: Filter candidate beats to get a more regular beat

            return filteredBeats;
        }

        private List<double> ComputeDynamicThreshold(IList<double> signal, int windowSize, int minPeakCount, int maxPeakCount)
        {
            var thresholdPoints = new List<Point2D>();
            var startIdx = 0;
            while (startIdx+windowSize <= signal.Count)
            {
                var valuesInWindow = signal.SubArray(startIdx, windowSize);
                var orderedValues = valuesInWindow.OrderByDescending(x => x).ToList();
                var minPeakValue = orderedValues[minPeakCount-1];
                var maxPeakValue = orderedValues[maxPeakCount-1];
                var combinedThreshold = 0.8 * minPeakValue + 0.2 * maxPeakValue;
                thresholdPoints.Add(new Point2D(startIdx + windowSize/2, combinedThreshold));
                startIdx += windowSize / 2;
            }
            thresholdPoints.Add(new Point2D(double.NegativeInfinity, thresholdPoints.First().Y));
            thresholdPoints.Add(new Point2D(double.PositiveInfinity, thresholdPoints.Last().Y));
            var continuousThreshold = new ContinuousLine2D(thresholdPoints);
            var dynamicThreshold = Enumerable.Range(0, signal.Count)
                .Select(idx => continuousThreshold.ValueAtX(idx))
                .ToList();
            return dynamicThreshold;
        }
    }
}
