
using System.Threading;
using Riff.Framework;

namespace Riff
{
    public abstract class SpeechHandler : AbstractSpeechHandler
    {
        #region Constructor(s)
        public SpeechHandler(ISpeechContext speechContext) : base(speechContext)
        {
        }
        #endregion
    }
}
