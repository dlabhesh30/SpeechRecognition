using System.Collections.Concurrent;
using Google.Protobuf;
using NAudio.Wave;

namespace Riff.Framework
{
    public interface IMicrophoneContext
    {
        WaveInEvent StartListening();

        BlockingCollection<ByteString> UnprocessedMicrophoneBuffer();

        int SampleRate();

        int ChannelCount();

        int BytesPerSample();
    }
}
