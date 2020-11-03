using System;
using System.Collections.Generic;
using System.Globalization;
using System.Speech.Synthesis;

namespace Riff
{
    enum AvailableVoices
    {
        ZiraPro_en_US,
        Helen_en_US,
        Hayley_en_AU, 
        Hazel_en_GB, 
        Heather_en_CA
    };

    public class SpeechContext
    {
        #region Private Data
        private SpeechSynthesizer m_speechSynthesizer = null;
        private CultureInfo m_cultureInfo = null;
        private Dictionary<AvailableVoices, KeyValuePair<int, string>> m_availableVoices;
        #endregion

        #region Constructor(s)
        public SpeechContext()
        {
            PopulateAvailableVoices();
            var voice = m_availableVoices[AvailableVoices.Helen_en_US];
            m_speechSynthesizer = new SpeechSynthesizer();
            m_cultureInfo = new CultureInfo(voice.Value, true);
            m_speechSynthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, voice.Key, m_cultureInfo);
        }
        #endregion

        #region Public method(s)
        public SpeechSynthesizer SpeechSynthesizer
        {
            get
            {
                return m_speechSynthesizer;
            }
        }

        public void Speak(String phrase, List<String> timeStrings = null)
        {
            var speechBuilder = new PromptBuilder();
            speechBuilder.AppendText(phrase);

            if (timeStrings != null && timeStrings.Count > 0)
            {
                foreach (var time in timeStrings)
                {
                    speechBuilder.AppendSsmlMarkup(time);
                }
            }
            m_speechSynthesizer.Speak(speechBuilder);
        }

        public void NoOptionAvailable()
        {
            this.Speak("There is currently no command for this");
        }
        #endregion

        #region Private method(s)
        private void PopulateAvailableVoices()
        {
            m_availableVoices = new Dictionary<AvailableVoices, KeyValuePair<int, string>>();
            m_availableVoices.Add(AvailableVoices.Hayley_en_AU, new KeyValuePair<int, string>(0, "en-AU"));
            m_availableVoices.Add(AvailableVoices.Heather_en_CA, new KeyValuePair<int, string>(0, "en-CA"));
            m_availableVoices.Add(AvailableVoices.Hazel_en_GB, new KeyValuePair<int, string>(0, "en-GB"));
            m_availableVoices.Add(AvailableVoices.ZiraPro_en_US, new KeyValuePair<int, string>(2, "en-US"));
            m_availableVoices.Add(AvailableVoices.Helen_en_US, new KeyValuePair<int, string>(0, "en-US"));
        }
        #endregion
    }
}
