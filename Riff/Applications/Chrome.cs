using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff
{
    public class Chrome : AbstractApplicationContext
    {
        public Chrome()
        {
            m_applicationName = "Chrome";
            this.SetApplicationPath();
            m_additionalAppStartMessage = "Happy browsing";
        }
    }
}
