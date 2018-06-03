using System.IO;
using System.Linq;
using Commons;
using OggVorbisEncoder;

namespace BeatSaberSongGenerator.AudioProcessing
{
    /// <summary>
    /// Converts any audio supported by NAudio and converts to .ogg-format
    /// </summary>
    public class AudioToOggConverter
    {
        public void Convert(string inputFile, string outputFile)
        {
            using (var outputStream = File.Create(outputFile))
            {
                var channels = AudioSampleReader.ReadChannels(inputFile, out var waveFormat);

                var oggStream = new OggStream(StaticRandom.Rng.Next());

                // Build header
                var headerBuilder = new HeaderPacketBuilder();
                var audioInfo = VorbisInfo.InitVariableBitRate(channels.Count, waveFormat.SampleRate, 0.1f);
                var infoPacket = headerBuilder.BuildInfoPacket(audioInfo);
                var commentsPacket = headerBuilder.BuildCommentsPacket(new Comments());
                var booksPacket = headerBuilder.BuildBooksPacket(audioInfo);
                oggStream.PacketIn(infoPacket);
                oggStream.PacketIn(commentsPacket);
                oggStream.PacketIn(booksPacket);
                WriteHeader(oggStream, outputStream);

                // Store audio
                var processingState = ProcessingState.Create(audioInfo);
                var channelArrays = channels.Select(channel => channel.ToArray()).ToArray();
                processingState.WriteData(channelArrays, channelArrays[0].Length);
                processingState.WriteEndOfStream();
                WriteAudio(oggStream, outputStream, processingState);
            }
        }

        private static void WriteHeader(OggStream oggStream, Stream outputStream)
        {
            while (oggStream.PageOut(out var page, true))
            {
                outputStream.Write(page.Header, 0, page.Header.Length);
                outputStream.Write(page.Body, 0, page.Body.Length);
            }
        }

        private static void WriteAudio(
            OggStream oggStream, 
            Stream outputStream, 
            ProcessingState processingState)
        {
            while (!oggStream.Finished && processingState.PacketOut(out var packet))
            {
                oggStream.PacketIn(packet);

                while (!oggStream.Finished && oggStream.PageOut(out var page, false))
                {
                    outputStream.Write(page.Header, 0, page.Header.Length);
                    outputStream.Write(page.Body, 0, page.Body.Length);
                }
            }
        }
    }
}
