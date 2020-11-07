using System.Collections.Concurrent;
using Google.Protobuf;
using NAudio.Wave;
using Riff.Framework;

namespace Riff.RecognitionProvider
{
    public class MicrophoneContext : IMicrophoneContext
    {
        #region Private data
        private const int m_sampleRate = 16000;
        private const int m_channelCount = 1;
        private const int m_bytesPerSample = 2;
        private readonly BlockingCollection<ByteString> m_unprocessedMicrophoneBuffer;
        #endregion
        
        #region Contructor(s)
        public MicrophoneContext()
        {
            m_unprocessedMicrophoneBuffer = new BlockingCollection<ByteString>();
        }
        #endregion

        #region Public method(s)
        public WaveInEvent StartListening()
        {
            var waveIn = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(m_sampleRate, m_channelCount)
            };
            waveIn.DataAvailable += (sender, args) =>
            m_unprocessedMicrophoneBuffer.Add(ByteString.CopyFrom(args.Buffer, 0, args.BytesRecorded));
            waveIn.StartRecording();
            return waveIn;
        }

        public BlockingCollection<ByteString> UnprocessedMicrophoneBuffer()
        {
            return m_unprocessedMicrophoneBuffer;
        }

        public int SampleRate()
        {
            return m_sampleRate;
        }

        public int ChannelCount()
        {
            return m_channelCount;
        }

        public int BytesPerSample()
        {
            return m_bytesPerSample;
        }
        #endregion
    }
}
