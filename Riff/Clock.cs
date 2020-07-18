using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff
{
    class Clock : SpeechHandler
    {
        #region Public method(s)
        public override void HandleSpeechRequest(string speech)
        {
            if (speech.Contains("TIME"))
            {
                CurrentTime();
            }
            else
            {
                this.PassRequestHandling(speech);
            }
        }
        #endregion

        #region Private method(s)
        private void CurrentTime()
        {
            var timeString = DateTime.Now.ToShortTimeString();
            var timePrefix = "The time is: ";

            m_speechContext.Speak(timePrefix, new List<String>() { " <say-as interpret-as=\"time\">" + timeString + "</say-as>" });
        }
        #endregion

    }
}
