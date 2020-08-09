using System.Collections.Generic;
using System.IO;

namespace Riff
{
    class GrammarContext
    {
        #region Private Data
        private List<string> m_grammarPhrases = null;
        private RiffConfigurableSettings m_riffConfigurableSettings = null;
        #endregion

        #region Constructor(s)
        public GrammarContext()
        {
            m_grammarPhrases = new List<string>();
            m_riffConfigurableSettings = Bootstrapper.ResolveType<RiffConfigurableSettings>();
        }
        #endregion

        #region Public method(s)
        public List<string> getPhrases()
        {
            string[] phrases = File.ReadAllLines(m_riffConfigurableSettings.GrammarPath);
            var parsedPhrases = new List<string>();
            foreach (var phrase in phrases)
            {
                if (phrase != string.Empty)
                {
                    parsedPhrases.Add(phrase);
                    continue;
                }
                parsedPhrases.Add("Empty");
            }

            parsedPhrases.AddRange(ReminderGrammar());

            return parsedPhrases;
        }
        #endregion

        #region Private method(s)
        private List<string> ReminderGrammar()
        {
            var baseReminderText = "SET A REMINDER FOR ";
            var phrases = new List<string>();

            // Reminder in minutes / seconds / hours
            for (int i = 0; i < 60; i++)
            {
                phrases.Add(baseReminderText + i.ToString() + " MINUTES");
                phrases.Add(baseReminderText + i.ToString() + " HOURS");
                phrases.Add(baseReminderText + i.ToString() + " SECONDS");
            }
            
            return phrases;
        }
        #endregion
    }
}
