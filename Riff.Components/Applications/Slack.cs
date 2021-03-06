﻿using Riff.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.Components
{
    public class Slack : AbstractApplicationContext
    {
        public Slack(IRiffConfigurableSettings riffConfigurableSettings, ISpeechContext speechContext)
            : base(riffConfigurableSettings, speechContext)
        {
            m_applicationName = "Slack";
            this.SetApplicationPath();
            m_additionalAppStartMessage = "Message away";
        }

    }
}
