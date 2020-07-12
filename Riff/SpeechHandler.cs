
using System.Threading;

namespace Riff
{
    public abstract class SpeechHandler
    {
        #region Protected Data
        protected SpeechHandler m_speechSuccessor = null;
        protected SpeechContext m_speechContext = null;
        #endregion

        #region Constructor(s)
        public SpeechHandler()
        {
            m_speechContext = Bootstrapper.ResolveType<SpeechContext>();
        }
        #endregion

        #region Public method(s)
        public abstract void HandleSpeechRequest(string speech);

        public void SetSuccessor(SpeechHandler speechSuccessor)
        {
            this.m_speechSuccessor = speechSuccessor;
        }
        #endregion

        #region Protected method(s)
        protected void PassRequestHandling(string speech)
        {
            if (null != m_speechSuccessor)
            {
                m_speechSuccessor.HandleSpeechRequest(speech);
            }
            else
            {
                NoCommandAvailable();
            }
        }
        #endregion

        #region Private method(s)
        private void NoCommandAvailable()
        {
            Thread noOptAvail = new Thread(new ThreadStart(() => m_speechContext.NoOptionAvailable()));
            noOptAvail.IsBackground = true;
            noOptAvail.Start();
        }
        #endregion
    }
}
