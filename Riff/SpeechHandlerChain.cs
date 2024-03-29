﻿using Riff.Framework;
using Riff.Components;
using Riff.Components.Google;
using Riff.Components.ChatGPT;

namespace Riff
{
    public class SpeechHandlerChain : ISpeechHandlerChain
    {

        #region Private Data
        private Greetings m_greetings = null;
        private Email m_email = null;
        private Calendar m_calendar = null;
        private Clock m_clock = null;
        private Weather m_weather = null;
        private GeneralSearch m_generalSearch = null;
        private RiffSystemOperations m_riffSystemOperations = null;
        private BatteryStatus m_batteryStatus = null;
        private Outlook m_outlook = null;
        private AbstractApplicationContext m_chrome = null;
        private AbstractApplicationContext m_slack = null;
        private AbstractApplicationContext m_word = null;
        private AbstractApplicationContext m_powerpoint = null;
        private AbstractApplicationContext m_excel = null;
        #endregion

        #region Constructor(s)
        public SpeechHandlerChain()
        {
            ResolveTypes();
        }
        #endregion

        #region Public method(s)
        public void SetupHandlerChain()
        {
            m_greetings.SetSuccessor(m_riffSystemOperations);
            m_riffSystemOperations.SetSuccessor(m_weather);
            m_weather.SetSuccessor(m_clock);
            m_clock.SetSuccessor(m_outlook);
            m_outlook.SetSuccessor(m_calendar);
            m_calendar.SetSuccessor(m_chrome);
            m_chrome.SetSuccessor(m_slack);
            m_slack.SetSuccessor(m_word);
            m_word.SetSuccessor(m_powerpoint);
            m_powerpoint.SetSuccessor(m_excel);
            m_excel.SetSuccessor(m_batteryStatus);
            m_batteryStatus.SetSuccessor(m_generalSearch);
        }

        public void HandleSpeechRequest(string speech)
        {
            m_greetings.HandleSpeechRequest(speech.ToUpper().Trim());
        }
        #endregion

        #region Private method(s)
        private void ResolveTypes()
        {
            m_weather = Bootstrapper.ResolveType<Weather>();
            m_email = Bootstrapper.ResolveType<Email>();
            m_calendar = Bootstrapper.ResolveType<Calendar>();
            m_greetings = Bootstrapper.ResolveType<Greetings>();
            m_riffSystemOperations = Bootstrapper.ResolveType<RiffSystemOperations>();
            m_clock = Bootstrapper.ResolveType<Clock>();
            m_outlook = Bootstrapper.ResolveType<Outlook>();
            m_chrome = Bootstrapper.ResolveType<Chrome>();
            m_slack = Bootstrapper.ResolveType<Slack>();
            m_word = Bootstrapper.ResolveType<Word>();
            m_powerpoint = Bootstrapper.ResolveType<Powerpoint>();
            m_excel = Bootstrapper.ResolveType<Excel>();
            m_batteryStatus = Bootstrapper.ResolveType<BatteryStatus>();
            m_generalSearch = Bootstrapper.ResolveType<GeneralSearch>();
        }
        #endregion
    }
}
