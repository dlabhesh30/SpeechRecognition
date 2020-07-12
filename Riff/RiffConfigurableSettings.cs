using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace Riff
{
    public class RiffConfigurableSettings
    {
        #region Private Data
        private NameValueCollection m_riffSettingsSection;
        private IDictionary<String, String> m_supportedApplications = null;
        #endregion

        #region Private Constructor(s)
        public RiffConfigurableSettings()
        {
            m_riffSettingsSection = (NameValueCollection)ConfigurationManager.GetSection("riffConfigurableSettings");
            var applicationsSection = (NameValueCollection)ConfigurationManager.GetSection("riffSupportedApplications");
            m_supportedApplications = new Dictionary<String, String>();
            
            foreach (string key in applicationsSection.AllKeys)
            {
                m_supportedApplications.Add(key, applicationsSection[key]);
            }
        }
        #endregion

        #region Public methods
        public string GrammarPath
        {
            get
            {
                const string key = "Riff.GrammarPath";
                var grammarPath = ResolveSetting(key);
                return grammarPath;
            }
        }

        public IDictionary<String, String> SupportedApplications
        {
            get
            {
                return m_supportedApplications;
            }
        }
        #endregion

        #region Private Implementation
        private string ResolveSetting(string key)
        {
            var result = m_riffSettingsSection[key];
            if (string.IsNullOrEmpty(result))
            {
                throw new SettingsPropertyNotFoundException("Unable to find the key : " + key);
            }
            return result;
        }
        #endregion


    }
}