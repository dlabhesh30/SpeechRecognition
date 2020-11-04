using System;
using System.Collections.Generic;
using System.Speech.Synthesis;

namespace Riff.Framework
{
    public interface ISpeechContext
    {
        SpeechSynthesizer SpeechSynthesizer();

        void Speak(String phrase, List<String> timeStrings = null);

        void NoOptionAvailable();
    }
}
