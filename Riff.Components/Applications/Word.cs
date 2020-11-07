﻿using Riff.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff
{
    public class Word : AbstractApplicationContext
    {
        public Word(IRiffConfigurableSettings riffConfigurableSettings, ISpeechContext speechContext)
            : base(riffConfigurableSettings, speechContext)
        {
            m_applicationName = "Word";
            this.SetApplicationPath();
        }
    }
}