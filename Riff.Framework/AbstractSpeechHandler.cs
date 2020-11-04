using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Riff.Framework
{
    public abstract class AbstractSpeechHandler
    {
        #region Protected Data
        protected AbstractSpeechHandler m_speechSuccessor = null;
        protected ISpeechContext m_speechContext = null;
        #endregion

        #region Constructor(s)
        public AbstractSpeechHandler(ISpeechContext speechContext)
        {
            m_speechContext = speechContext;
        }
        #endregion
        
        #region Public method(s)
        public abstract void HandleSpeechRequest(string speech);

        public void SetSuccessor(AbstractSpeechHandler speechSuccessor)
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

        protected void NoCommandAvailable()
        {
            Thread noOptAvail = new Thread(new ThreadStart(() => m_speechContext.NoOptionAvailable()));
            noOptAvail.IsBackground = true;
            noOptAvail.Start();
        }
        #endregion
    }
}
