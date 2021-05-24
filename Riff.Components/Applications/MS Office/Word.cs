using Riff.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.Components
{
    public class Word : AbstractApplicationContext
    {
        #region Private Data
        private string m_baseApplicationPathName = "MSOffice";
        private string m_executableName = "WINWORD.exe";
        #endregion

        #region Constructor(s)
        public Word(IRiffConfigurableSettings riffConfigurableSettings, ISpeechContext speechContext)
            : base(riffConfigurableSettings, speechContext)
        {
            m_applicationName = "Word";
            this.SetCommonApplicationBasePathOverride(m_baseApplicationPathName, m_executableName);
            this.SetApplicationPath();
            SetAlternateAliasForApplciation();
        }
        #endregion

        #region Protected method(s)
        protected override void SetAlternateAliasForApplciation()
        {
            m_alternateApplicationAlias.Add("WORD DOCUMENT");
            m_alternateApplicationAlias.Add("WORD DOC");
            m_alternateApplicationAlias.Add("MS WORD");
            m_alternateApplicationAlias.Add("MICROSOFT WORD");
        }
        #endregion
    }
}
