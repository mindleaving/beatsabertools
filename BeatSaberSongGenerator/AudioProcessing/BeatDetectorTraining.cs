using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Commons;
using Commons.Extensions;
using Commons.IO;
using NWaves.Transforms;
using NWaves.Windows;
using SharpLearning.Containers.Matrices;
using SharpLearning.InputOutput.Csv;
using SharpLearning.InputOutput.Serialization;
using SharpLearning.Neural;
using SharpLearning.Neural.Layers;
using SharpLearning.Neural.Learners;
using SharpLearning.Neural.Loss;
using CsvWriter = SharpLearning.InputOutput.Csv.CsvWriter;

namespace BeatSaberSongGenerator.AudioProcessing
{
    public class BeatDetectorTraining
    {
        public const int Downsampling = 10;
        public const int SamplesBeforeBeat = 599;
        public const int SamplesAfterBeat = 1000;
        public const int InputSampleSize = SamplesBeforeBeat + 1 + SamplesAfterBeat;

        public void Train(Dictionary<string, string> songFileToBeatPositionHeaderMap, string beatPositionFilePath,
            string modelOutputFile)
        {
            const int SamplesBetweenBeats = (44100 / Downsampling) / (120 / 60); // For BPM = 120

            var trainingDataPath = Path.Combine(Path.GetDirectoryName(modelOutputFile), "trainingData.csv");
            var labelsPath = Path.Combine(Path.GetDirectoryName(modelOutputFile), "trainingLabels.csv");

            F64Matrix trainingDataMatrix;
            IList<double> labels;
            //if (!File.Exists(trainingDataPath) || !File.Exists(labelsPath))
            //{
                var beatPositionColumn = CsvReader.ReadColumns(beatPositionFilePath)
                    .ToDictionary(kvp => kvp.Key,
                        kvp => kvp.Value.Where(str => !string.IsNullOrEmpty(str)).Select(int.Parse).ToList());

                var trainingData = new List<IList<float>>();
                labels = new List<double>();
                foreach (var songFile in songFileToBeatPositionHeaderMap.Keys)
                {
                    var audioData = AudioSampleReader.ReadMonoSamples(songFile, out var sampleRate);
                    if (sampleRate != 44100)
                        throw new Exception();
                    var stft = new Stft(windowSize:4096, hopSize: 1024, window: WindowTypes.Hamming, fftSize: 128);
                    var spectrogram = stft.Spectrogram(audioData.ToArray());
                    var downsampledAudioData = AudioDownsampler.Downsample(audioData, Downsampling);
                    var beatPositionHeader = songFileToBeatPositionHeaderMap[songFile];
                    var beatPositions = beatPositionColumn[beatPositionHeader];
                    foreach (var beatPosition in beatPositions)
                    {
                        var downsampledBeatPosition = beatPosition / Downsampling;
                        if (downsampledBeatPosition + InputSampleSize >= downsampledAudioData.Count)
                            continue;
                        var beatSample = downsampledAudioData.SubArray(downsampledBeatPosition - SamplesBeforeBeat, InputSampleSize);

                        var nonBeatPosition = downsampledBeatPosition
                                              + (int) ((0.2 + 0.6 * StaticRandom.Rng.NextDouble()) * SamplesBetweenBeats);
                        if (nonBeatPosition + InputSampleSize >= downsampledAudioData.Count)
                            continue;
                        var nonBeatSample = downsampledAudioData.SubArray(nonBeatPosition-SamplesBeforeBeat, InputSampleSize);

                        trainingData.Add(beatSample);
                        labels.Add(1);
                        trainingData.Add(nonBeatSample);
                        labels.Add(0);
                    }
                }
                return;

                trainingDataMatrix = new F64Matrix(
                    trainingData.SelectMany(row => row).Select(x => (double) x).ToArray(),
                    trainingData.Count, InputSampleSize);
                var featureColumnNames = Enumerable.Range(0, InputSampleSize)
                    .ToDictionary(idx => $"S{idx}", idx => idx);
                new CsvWriter(() => new StreamWriter(trainingDataPath))
                    .Write(trainingDataMatrix.EnumerateCsvRows(featureColumnNames));
                File.WriteAllLines(
                    labelsPath,
                    labels.Select(x => x.ToString("F0")));
            //}
            //else
            //{
            //    labels = File.ReadAllLines(labelsPath).Select(double.Parse).ToList();
            //    trainingDataMatrix = new CsvParser(() => new StreamReader(trainingDataPath))
            //        .EnumerateRows()
            //        .ToF64Matrix();
            //}


            var neuralNetwork = new NeuralNet();
            neuralNetwork.Add(new InputLayer(InputSampleSize, 1, 1));
            neuralNetwork.Add(new Conv2DLayer(32, 1, 16, 4));
            neuralNetwork.Add(new MaxPool2DLayer(8, 1, 4));
            neuralNetwork.Add(new Conv2DLayer(32, 1, 16, 4));
            neuralNetwork.Add(new MaxPool2DLayer(8, 1, 4));
            //neuralNetwork.Add(new Conv2DLayer(16, 1, 8, 4));
            //neuralNetwork.Add(new MaxPool2DLayer(8, 1, 4));
            neuralNetwork.Add(new DenseLayer(25));
            neuralNetwork.Add(new DenseLayer(10));
            neuralNetwork.Add(new DropoutLayer(0.4));
            neuralNetwork.Add(new SoftMaxLayer(2));
            var learner = new ClassificationNeuralNetLearner(neuralNetwork, new LogLoss());
            var model = learner.Learn(trainingDataMatrix, labels.ToArray());
            var binarySerializer = new GenericBinarySerializer();
            binarySerializer.Serialize(model, () => new StreamWriter(modelOutputFile));
        }
    }
}
