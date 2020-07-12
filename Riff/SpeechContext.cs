using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;

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
        private int m_originalVolume = 50;
        private SpeechRecognitionEngine m_speechRecognitionEngine = null;
        private List<String> m_grammarPhrases = null;
        private RiffConfigurableSettings m_riffConfigurableSettings = null;
        private CultureInfo m_cultureInfo = null;
        Dictionary<AvailableVoices, KeyValuePair<int, string>> m_availableVoices;
        
        #endregion

        #region Constructor(s)
        public SpeechContext()
        {
            PopulateAvailableVoices();
            var voice = m_availableVoices[AvailableVoices.Helen_en_US];
            m_speechSynthesizer = new SpeechSynthesizer();
            m_speechRecognitionEngine = new SpeechRecognitionEngine();
            m_cultureInfo = new CultureInfo(voice.Value, true);
            m_grammarPhrases = new List<String>();
            m_riffConfigurableSettings = Bootstrapper.ResolveType<RiffConfigurableSettings>();

            SetCustomGrammar();
            //Set output, load grammar and set up speech recognisition event handler
            m_speechRecognitionEngine.SetInputToDefaultAudioDevice();
            m_speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);

            m_speechSynthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, voice.Key, m_cultureInfo);
        }
        #endregion

        #region Public method(s)

        public SpeechRecognitionEngine SpeechEngine
        {
            get
            {
                return m_speechRecognitionEngine;
            }
        }

        public void SetDictationGrammar()
        {
            var defaultDictationGrammar = new DictationGrammar();
            defaultDictationGrammar.Name = "default dictation";
            defaultDictationGrammar.Enabled = true;
            m_speechRecognitionEngine.LoadGrammar(defaultDictationGrammar);
        }

        public void SetCustomGrammar()
        {
            m_grammarPhrases = getPhrases();
            m_speechRecognitionEngine.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(m_grammarPhrases.ToArray()))));
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

        public void AskForFilename()
        {
            this.Speak("What file are you looking for?");
        }
        
        public void Appointment(string[] whatToSay)
        {
            var date = whatToSay[2].Split('/');
            var newDate = date[1] + "/" + date[0];
            var time = whatToSay[3].Split(':');
            var newTime = time[0] + ":" + time[1];
            
            var appointmentText = new StringBuilder();
            appointmentText.Append("Appointment " + whatToSay[0]);
            appointmentText.Append(", subject is: " + whatToSay[1]);
            appointmentText.Append(", at " + whatToSay[3]);
            appointmentText.Append(", on");

            var timeString = new List<String>(); 
            timeString.Add("<say-as interpret-as=\"date_md\">" + newDate + "</say-as>");
            timeString.Add(" <say-as interpret-as=\"time\">" + newTime + "</say-as>");

            this.Speak(appointmentText.ToString(), timeString);
            
        }

        public void AppointmentError()
        {
            this.Speak("No Appointments found for the next 7 days.");
        }

        public void SearchFor()
        {
            this.Speak("What do you want to search for?");
        }

        public void CurrentTime()
        {
            var timeString = DateTime.Now.ToShortTimeString();
            var timePrefix = "The time is: ";

            this.Speak(timePrefix, new List<String>() { " <say-as interpret-as=\"time\">" + timeString + "</say-as>" });
        }

        public void CurrentDate()
        {
            string dateString = DateTime.Now.ToShortDateString();
            var datePrefix = "The date is: ";
            this.Speak(datePrefix, new List<String>() { " <say-as interpret-as=\"date_md\">" + dateString + "</say-as>" });
        }

        public void WeatherForecast(string weather)
        {
            this.Speak(weather);
        }

        public void Weather(string weather)
        {
            this.Speak(weather);
        }

        public void Loading()
        {
            this.Speak("Loading, Please hang tight");
        }

        
        public void Mute(bool mute)
        {
            if (mute)
            {
                this.Speak("Muting");
                m_originalVolume = m_speechSynthesizer.Volume;
                m_speechSynthesizer.Volume = 0;
            }
            else
            {
                m_speechSynthesizer.Volume = m_originalVolume;
                m_speechSynthesizer.Speak("Volume levels restored");
            }
        }


        public void Volume(bool volumeDown)
        {
            int volume = m_speechSynthesizer.Volume;
            if (volumeDown)
            {
                if (m_speechSynthesizer.Volume == 0 || (m_speechSynthesizer.Volume - 20) < 0)
                {
                    m_speechSynthesizer.Volume = 20;
                    m_speechSynthesizer.Speak("Shhh...Muted");
                    m_speechSynthesizer.Volume = 0;
                }
                else
                {
                    m_speechSynthesizer.Volume -= 20;
                    m_speechSynthesizer.Speak("Getting quieter, m_volume decreased");
                }
            }
            else
            {
                if (m_speechSynthesizer.Volume == 100 || (m_speechSynthesizer.Volume + 20) > 100)
                {
                    m_speechSynthesizer.Speak("I am at my max m_volume, I would recommend not trying to go any louder");
                }
                else
                {
                    m_speechSynthesizer.Volume += 20;
                    m_speechSynthesizer.Speak("Yes, Volume increased");
                }
            }
        }

        public void TooManyRecipients()
        {
            this.Speak("Maximum number of recipients added");
        }

        public void Closing()
        {
            m_speechSynthesizer.SpeakAsyncCancelAll();
            m_speechSynthesizer.Dispose();
        }
        #endregion

        #region Private method(s)
        public List<String> getPhrases()
        {
            string[] phrases = File.ReadAllLines(m_riffConfigurableSettings.GrammarPath);
            var parsedPhrases = new List<String>();
            foreach (string phrase in phrases)
            {
                if (phrase != string.Empty)
                {
                    parsedPhrases.Add(phrase);
                    continue;
                }
                parsedPhrases.Add("Empty");
            }
            return parsedPhrases;
        }

        void PopulateAvailableVoices()
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
