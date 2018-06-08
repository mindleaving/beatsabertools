using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberSongGenerator.Objects;
using Commons.DataProcessing;
using Commons.Extensions;
using Commons.Mathematics;
using Commons.Physics;
using NWaves.Transforms;
using NWaves.Windows;

namespace BeatSaberSongGenerator.AudioProcessing
{
    public class BeatDetectorResult
    {
        public BeatDetectorResult(
            double beatsPerMinute, 
            int beatsPerBar,
            List<Beat> detectedBeats, 
            List<Beat> regularBeats, 
            List<SongIntensity> songIntensities = null)
        {
            BeatsPerMinute = beatsPerMinute;
            BeatsPerBar = beatsPerBar;
            DetectedBeats = detectedBeats;
            RegularBeats = regularBeats;
            SongIntensities = songIntensities;
        }

        public double BeatsPerMinute { get; }
        public int BeatsPerBar { get; set; }
        public List<Beat> DetectedBeats { get; }
        public List<Beat> RegularBeats { get; }
        public List<SongIntensity> SongIntensities { get; }
    }
    public class BeatDetector
    {
        private const double BeatInaccuracyTolerance = 0.05;
        private readonly WindowClusterer windowClusterer;

        public BeatDetector()
        {
            windowClusterer = new WindowClusterer();
        }

        /// <summary>
        /// Takes an audio signal, detects the beats and returns a list of all times (in seconds) where a beat is located
        /// </summary>
        /// <param name="signal">Expected to be normalized to -1 to 1</param>
        /// <param name="sampleRate">Sample rate of signal</param>
        /// <returns>Sample indices of beats</returns>
        public BeatDetectorResult DetectBeats(IList<float> signal, int sampleRate)
        {
            var stftWindowSize = 4096;
            var stepSize = 1024;
            var lowerLimit = 0.1 * sampleRate;
            var upperLimit = 1.0 * sampleRate;

            var stft = new Stft(windowSize: stftWindowSize, hopSize: stepSize, window: WindowTypes.Hamming);
            var spectrogram = stft.Spectrogram(signal.ToArray());
            var windowPositions = SequenceGeneration
                .Linspace(stftWindowSize / 2.0, signal.Count-stftWindowSize/2.0, spectrogram.Count)
                .ToList();

            var fftMagnitudeIncreaseSeries = ComputeFftMagnitudeIncreaseSeries(spectrogram, windowPositions);
            var candidateBeats = FindCandidateBeats(fftMagnitudeIncreaseSeries, sampleRate, stepSize, out var songIntensity);
            var filteredBeats = FilterBeats(candidateBeats, lowerLimit);
            // TODO: Filter candidate beats to get a more regular beat
            var beatsPerMinute = DetermineBeatsPerMinute(filteredBeats, sampleRate, upperLimit);
            var regularBeats = GenerateRegularBeats(filteredBeats, beatsPerMinute, sampleRate, signal.Count);

            var beatsPerBar = 4;
            return new BeatDetectorResult(beatsPerMinute, beatsPerBar, filteredBeats, regularBeats, songIntensity);
        }

        private List<Beat> GenerateRegularBeats(List<Beat> detectedBeats, double beatsPerMinute, int sampleRate, int totalSampleCount)
        {
            var expectedBeatSeparationInSeconds = 60 / beatsPerMinute;
            var expectedBeatSeparationInSamples = (int)Math.Round(expectedBeatSeparationInSeconds * sampleRate);
            var expectedBeats = detectedBeats
                .Zip(detectedBeats.Skip(1), (b1, b2) => new
                {
                    Separation = (b2.SampleIndex - b1.SampleIndex) / (double) sampleRate,
                    SampleIndex1 = b1.SampleIndex,
                    SampleIndex2 = b2.SampleIndex
                })
                .Where(x => (x.Separation - expectedBeatSeparationInSeconds).Abs() <  BeatInaccuracyTolerance)
                .SelectMany(x => new[] { x.SampleIndex1, x.SampleIndex2})
                .Distinct()
                .ToList();
            var offset = expectedBeats
                .Select(sampleIdx => (double)sampleIdx.Modulus(expectedBeatSeparationInSamples))
                .Median();
            var regularBeatSampleIndices = SequenceGeneration.FixedStep(offset, totalSampleCount, expectedBeatSeparationInSamples);
            var regularBeats = regularBeatSampleIndices
                .Select(sampleIdx => new Beat
                {
                    SampleIndex = (int) Math.Round(sampleIdx),
                    Strength = 0
                })
                .ToList();
            return regularBeats;
        }

        private static List<Beat> FilterBeats(List<Beat> candidateBeats, double lowerLimit)
        {
            var filteredBeats = new List<Beat>();
            var lastBeatPosition = 0;
            foreach (var candidateBeat in candidateBeats)
            {
                if (candidateBeat.SampleIndex - lastBeatPosition < lowerLimit)
                    continue;
                //if(candidateBeatPosition - lastBeatPosition > upperLimit)
                //    filteredBeats.Add((candidateBeatPosition + lastBeatPosition) / 2);
                filteredBeats.Add(candidateBeat);
                lastBeatPosition = candidateBeat.SampleIndex;
            }

            return filteredBeats;
        }

        private List<Beat> FindCandidateBeats(IList<Point2D> fftMagnitudeIncreaseSeries, int sampleRate, int stepSize, out List<SongIntensity> songIntensities)
        {
            var thresholdWindowSize = (int) Math.Floor(1.5 * sampleRate / stepSize); // Corresponds to 1.5 seconds
            var dynamicThreshold = ComputeDynamicThreshold(
                fftMagnitudeIncreaseSeries.Select(p => p.Y).ToList(),
                thresholdWindowSize, 2, 4);
            var maxValue = fftMagnitudeIncreaseSeries.Max(p => p.Y);
            var movingAverageWindowSize = 1.0 * sampleRate;
            var averagedSignal = fftMagnitudeIncreaseSeries
                .MedianFilter(movingAverageWindowSize)
                .ToList();
            var averagedSignalMax = averagedSignal.Max(p => p.Y);
            songIntensities = averagedSignal.Select(p => new SongIntensity((int) p.X, p.Y / averagedSignalMax)).ToList();
            var candidateBeats = new List<Beat>();
            for (var pointIdx = 0; pointIdx < fftMagnitudeIncreaseSeries.Count; pointIdx++)
            {
                var timePoint = fftMagnitudeIncreaseSeries[pointIdx];
                var averageValue = averagedSignal[pointIdx].Y;
                var threshold = dynamicThreshold[pointIdx];
                if (timePoint.Y < threshold)
                    continue;
                var beat = new Beat
                {
                    SampleIndex = (int) timePoint.X,
                    Strength = (timePoint.Y - averageValue) / maxValue
                };
                candidateBeats.Add(beat);
            }

            return candidateBeats;
        }

        private static List<Point2D> ComputeFftMagnitudeIncreaseSeries(List<float[]> spectrogram, List<double> windowPositions)
        {
            const double IncreaseThreshold = 1e-5;

            var fftMagnitudeIncreaseSeries = new List<Point2D>();
            for (int stepIdx = 1; stepIdx < spectrogram.Count; stepIdx++)
            {
                var windowCenterSample = windowPositions[stepIdx];
                var previousFft = spectrogram[stepIdx - 1];
                var currentFft = spectrogram[stepIdx];

                var magnitudeIncreaseCount = 0;
                for (int frequencyBinIdx = 0; frequencyBinIdx < currentFft.Length; frequencyBinIdx++)
                {
                    var isIncrease = currentFft[frequencyBinIdx] - previousFft[frequencyBinIdx] > IncreaseThreshold;
                    if (isIncrease)
                        magnitudeIncreaseCount++;
                }

                fftMagnitudeIncreaseSeries.Add(new Point2D(windowCenterSample, magnitudeIncreaseCount));
            }

            return fftMagnitudeIncreaseSeries;
        }

        private double DetermineBeatsPerMinute(IReadOnlyCollection<Beat> filteredBeats, int sampleRate, double upperLimit)
        {
            var beatLengths = filteredBeats
                .Zip(filteredBeats.Skip(1), (idx1, idx2) => idx2.SampleIndex - idx1.SampleIndex)
                .Where(beatLength => beatLength < upperLimit)
                .ToList();
            var clusterWindowSize = (int) Math.Ceiling(BeatInaccuracyTolerance * sampleRate);
            var beatLengthClusters = windowClusterer.Cluster(beatLengths, x => x, clusterWindowSize, 1);
            var largestBeatLengthCluster = beatLengthClusters.MaximumItem(c => c.Items.Count);
            var averageBeatLength = largestBeatLengthCluster.Items.Average();
            var beatsPerMinute = sampleRate * 60 / averageBeatLength;
            if (beatsPerMinute < 100)
                beatsPerMinute *= 2;
            return beatsPerMinute;
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

    public class Beat
    {
        public int SampleIndex { get; set; }
        public double Strength { get; set; }
    }
}
