using BeatSaberSongGenerator.AudioProcessing;
using NUnit.Framework;

namespace BeatSabeSonglGeneratorTest
{
    [TestFixture]
    public class AudioToOggConverterTest
    {
        [Test]
        public void ConversionTest()
        {
            var sut = new AudioToOggConverter();
            var inputFilePath = @"C:\Users\Jan\Music\Dimitri Vegas - Ocarina.mp3";
            var outputFilePath = @"C:\Users\Jan\Music\Dimitri Vegas - Ocarina.ogg";
            sut.Convert(inputFilePath, outputFilePath);
        }
    }
}
