using Riff.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.Components
{
    public class Excel : AbstractApplicationContext
    {
        #region Private Data
        private string m_baseApplicationPathName = "MSOffice";
        private string m_executableName = "EXCEL.exe";
        #endregion

        #region Constructor(s)
        public Excel(IRiffConfigurableSettings riffConfigurableSettings, ISpeechContext speechContext)
            : base(riffConfigurableSettings, speechContext)
        {
            m_applicationName = "Excel";
            this.SetCommonApplicationBasePathOverride(m_baseApplicationPathName, m_executableName);
            this.SetApplicationPath();
        }
        #endregion
    }
}
