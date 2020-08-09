using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Riff
{
    class Clock : SpeechHandler
    {

        #region Private Data
        System.Timers.Timer m_timer = null;
        int m_timerInterval = 0;
        string m_timerIntervalType = "";
        #endregion

        public Clock()
        {
            m_timer = new System.Timers.Timer();
            m_timer.Enabled = false;
            m_timer.AutoReset = true;
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(OnRemiderInvoked);
        }

        #region Public method(s)
        public override void HandleSpeechRequest(string speech)
        {
            if (speech.Contains("TIME"))
            {
                CurrentTime();
            }
            else
            if (speech.Contains("REMINDER"))
            {
                SetReminder(speech);
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

        private void SetReminder(string speech)
        {
            var splitString = speech.Split(' ');
            m_timerInterval = Int32.Parse(splitString[splitString.Length - 2]);
            m_timerIntervalType = splitString[splitString.Length - 1];
            m_timer.Enabled = true;

            switch (m_timerIntervalType)
            {
                case "MINUTES":
                    m_timer.Interval = TimeSpan.FromMinutes(m_timerInterval).TotalMilliseconds;
                    break;
                case "HOURS":
                    m_timer.Interval = TimeSpan.FromHours(m_timerInterval).TotalMilliseconds;
                    break;
                case "SECONDS":
                    m_timer.Interval = TimeSpan.FromSeconds(m_timerInterval).TotalMilliseconds;
                    break;
            }
            
            m_timer.Start();
        }

        private void OnRemiderInvoked(object sender, ElapsedEventArgs e)
        {
            m_timer.Enabled = false;
            m_timer.Stop();

            var message = "Hey, this is your reminder for " + m_timerInterval + " " + m_timerIntervalType;
            m_speechContext.Speak(message);
        }
        #endregion

    }
}
