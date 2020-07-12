using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff
{
    public class Slack : AbstractApplicationContext
    {
        public Slack()
        {
            m_applicationName = "Slack";
            this.SetApplicationPath();
            m_additionalAppStartMessage = "Message away";
        }

    }
}
