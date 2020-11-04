
using Google.Cloud.Speech.V1;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riff.Framework;

namespace Riff.RecognitionProvider
{
    public class GoogleRecognitionEngineProvider : IRecognitionEngineProvider
        {
            #region Private Data  
            private int m_bytesPerSecond;
            private readonly SpeechClient m_client;
            private readonly LinkedList<ByteString> m_speechUnprocessedBuffer = new LinkedList<ByteString>();
            private TimeSpan m_processingBufferStart;
            private SpeechClient.StreamingRecognizeStream m_rpcStream;
            private DateTime m_rpcStreamDeadline;
            private ValueTask<bool> m_serverResponseAvailableTask;
            private static readonly TimeSpan s_streamTimeLimit = TimeSpan.FromSeconds(290);
            private IMicrophoneContext m_microphoneContext;
            private readonly StreamingRecognizeRequest m_streamingRecognizeRequest;
            private readonly ISpeechHandlerChain m_speechHandlerChain;
            #endregion

            #region Constructor(s)
            public GoogleRecognitionEngineProvider(IMicrophoneContext microphoneContext, ISpeechHandlerChain speechHandlerChain)
            {
                m_client = SpeechClient.Create();
                m_microphoneContext = microphoneContext;
                m_bytesPerSecond = m_microphoneContext.SampleRate() *
                    m_microphoneContext.ChannelCount() *
                    m_microphoneContext.BytesPerSample();
                m_speechHandlerChain = speechHandlerChain;
                m_streamingRecognizeRequest = CreateStreamingRecognizeRequest();
            }
            #endregion

            #region Public method(s)
            public async Task<int> RecognizeSpeech()
            {
                await RunAsync();
                return 0;
            }
            #endregion

            #region Private method(s)

            private StreamingRecognizeRequest CreateStreamingRecognizeRequest()
            {
                return new StreamingRecognizeRequest
                {
                    StreamingConfig = new StreamingRecognitionConfig
                    {
                        Config = new RecognitionConfig
                        {
                            Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                            SampleRateHertz = m_microphoneContext.SampleRate(),
                            LanguageCode = "en-US",
                            MaxAlternatives = 1
                        },
                        InterimResults = false,
                    }
                };
            }

            private async Task RunAsync()
            {
                using (var microphone = m_microphoneContext.StartListening())
                {
                    while (true)
                    {
                        await StartStreamAsync();
                        // ProcessResponses will return false if it hears "exit" or "quit".
                        if (!ProcessResponses())
                        {
                            return;
                        }
                        await TransferMicrophoneChunkAsync();
                    }
                }
            }

            private async Task StartStreamAsync()
            {
                var now = DateTime.UtcNow;
                if (m_rpcStream != null && now >= m_rpcStreamDeadline)
                {
                    Console.WriteLine($"Closing stream before it times out");
                    await m_rpcStream.WriteCompleteAsync();
                    m_rpcStream.GrpcCall.Dispose();
                    m_rpcStream = null;
                }

                // If we have a valid stream at this point, we're fine.
                if (m_rpcStream != null)
                {
                    return;
                }
                // We need to create a new stream, either because we're just starting or because we've just closed the previous one.
                m_rpcStream = m_client.StreamingRecognize();
                m_rpcStreamDeadline = now + s_streamTimeLimit;
                m_processingBufferStart = TimeSpan.Zero;
                m_serverResponseAvailableTask = m_rpcStream.GetResponseStream().MoveNextAsync();
                await m_rpcStream.WriteAsync(m_streamingRecognizeRequest);

                Console.WriteLine($"Writing {m_speechUnprocessedBuffer.Count} chunks into the new stream.");
                foreach (var chunk in m_speechUnprocessedBuffer)
                {
                    await WriteAudioChunk(chunk);
                }
            }

            private bool ProcessResponses()
            {
                while (m_serverResponseAvailableTask.IsCompleted && m_serverResponseAvailableTask.Result)
                {
                    var response = m_rpcStream.GetResponseStream().Current;
                    m_serverResponseAvailableTask = m_rpcStream.GetResponseStream().MoveNextAsync();

                    // See if one of the results is a "final result". If so, we trim our
                    // processing buffer.
                    var finalResult = response.Results.FirstOrDefault(r => r.IsFinal);
                    if (finalResult != null)
                    {
                        string transcript = finalResult.Alternatives[0].Transcript;
                        Console.WriteLine($"Transcript: {transcript}");
                        m_speechHandlerChain.HandleSpeechRequest(transcript);
                        if (transcript.ToLowerInvariant().Contains("exit") ||
                            transcript.ToLowerInvariant().Contains("quit"))
                        {
                            return false;
                        }

                        TimeSpan resultEndTime = finalResult.ResultEndTime.ToTimeSpan();

                        // Rather than explicitly iterate over the list, we just always deal with the first
                        // element, either removing it or stopping.
                        int removed = 0;
                        while (m_speechUnprocessedBuffer.First != null)
                        {
                            var sampleDuration = TimeSpan.FromSeconds(m_speechUnprocessedBuffer.First.Value.Length / (double)m_bytesPerSecond);
                            var sampleEnd = m_processingBufferStart + sampleDuration;

                            // If the first sample in the buffer ends after the result ended, stop.
                            // Note that part of the sample might have been included in the result, but the samples
                            // are short enough that this shouldn't cause problems.
                            if (sampleEnd > resultEndTime)
                            {
                                break;
                            }
                            m_processingBufferStart = sampleEnd;
                            m_speechUnprocessedBuffer.RemoveFirst();
                            removed++;
                        }
                    }
                }
                return true;
            }

            private async Task TransferMicrophoneChunkAsync()
            {
                // This will block - but only for ~100ms, unless something's really broken.
                var chunk = m_microphoneContext.UnprocessedMicrophoneBuffer().Take();
                m_speechUnprocessedBuffer.AddLast(chunk);
                await WriteAudioChunk(chunk);
            }

            private Task WriteAudioChunk(ByteString chunk)
            {
                return m_rpcStream.WriteAsync(new StreamingRecognizeRequest { AudioContent = chunk });
            }
            #endregion
        }
}
