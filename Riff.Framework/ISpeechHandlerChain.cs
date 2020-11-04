

namespace Riff.Framework
{
    public interface ISpeechHandlerChain
    {
        void SetupHandlerChain();

        void HandleSpeechRequest(string speech);
    }

}
