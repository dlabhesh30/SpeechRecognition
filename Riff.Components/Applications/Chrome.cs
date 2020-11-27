using Riff.Framework;

namespace Riff.Components
{
    public class Chrome : AbstractApplicationContext
    {
        public Chrome(IRiffConfigurableSettings riffConfigurableSettings, ISpeechContext speechContext)
            : base(riffConfigurableSettings, speechContext)
        {
            m_applicationName = "Chrome";
            this.SetApplicationPath();
            m_additionalAppStartMessage = "Happy browsing";
        }
    }
}
