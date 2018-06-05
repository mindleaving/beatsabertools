using System.Collections.Generic;
using System.IO;
using System.Linq;
using Commons.Extensions;
using SharpLearning.InputOutput.Serialization;
using SharpLearning.Neural.Models;

namespace BeatSaberSongGenerator.AudioProcessing
{
    public class BeatDetector
    {
        private readonly ClassificationNeuralNetModel model;

        public BeatDetector()
        {
            var binarySerializer = new GenericBinarySerializer();
            model = binarySerializer.Deserialize<ClassificationNeuralNetModel>(() => new StreamReader(@"C:\Temp\beatDetectorModel.bin"));
        }

        /// <summary>
        /// Takes an audio signal, detects the beats and returns a list of all times (in seconds) where a beat is located
        /// </summary>
        /// <param name="signal">Expected to be normalized to -1 to 1</param>
        /// <returns>Time of beats</returns>
        public List<int> DetectBeats(IList<float> signal)
        {
            var downsampledSignal = AudioDownsampler.Downsample(signal, BeatDetectorTraining.Downsampling);
            var stride = 256;
            var inputWidth = BeatDetectorTraining.InputSampleSize;
            var startIdx = 0;
            var beatProbabilities = new List<BeatProbability>();
            while (startIdx + inputWidth < downsampledSignal.Count)
            {
                var inputData = downsampledSignal.SubArray(startIdx, inputWidth).Select(x => (double)x).ToArray();
                var probabilityPrediction = model.PredictProbability(inputData);
                var isBeatProbability = probabilityPrediction.Probabilities[2];
                var isNotBeatProbability = probabilityPrediction.Probabilities[1];
                var beatProbability = isBeatProbability / (isBeatProbability + isNotBeatProbability);
                beatProbabilities.Add(new BeatProbability(startIdx+BeatDetectorTraining.SamplesBeforeBeat, beatProbability));
                startIdx += stride;
            }
            File.WriteAllLines(
                @"C:\Temp\beatProbabilities.csv",
                beatProbabilities.Select(x => $"{x.SampleIdx*BeatDetectorTraining.Downsampling};{x.Probability:F4}"));
            return beatProbabilities
                .Where(x => x.Probability > 0.5)
                .Select(x => x.SampleIdx * BeatDetectorTraining.Downsampling)
                .ToList();
        }

        private class BeatProbability
        {
            public BeatProbability(int sampleIdx, double probability)
            {
                SampleIdx = sampleIdx;
                Probability = probability;
            }

            public int SampleIdx { get; }
            public double Probability { get; }
        }
    }
}
