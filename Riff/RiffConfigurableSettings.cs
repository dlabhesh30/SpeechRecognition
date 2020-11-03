using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace Riff
{
    public class RiffConfigurableSettings
    {
        #region Private Data
        private IDictionary<String, String> m_supportedApplications = null;
        #endregion

        #region Private Constructor(s)
        public RiffConfigurableSettings()
        {
            var applicationsSection = (NameValueCollection)ConfigurationManager.GetSection("riffSupportedApplications");
            m_supportedApplications = new Dictionary<String, String>();
            
            foreach (string key in applicationsSection.AllKeys)
            {
                m_supportedApplications.Add(key, applicationsSection[key]);
            }
        }
        #endregion

        #region Public methods
        public IDictionary<String, String> SupportedApplications
        {
            get
            {
                return m_supportedApplications;
            }
        }
        #endregion
    }
}