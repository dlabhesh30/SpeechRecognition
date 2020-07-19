﻿using System;
using System.Speech.Recognition;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Speech.Synthesis;

namespace Riff
{
    public class RiffSystemOperations : SpeechHandler
    {
        #region Private Data
        private RiffApplication m_riffWindow = null;
        private Greetings m_greetings = null;
        private SpeechSynthesizer m_speechSynthesizer = null;
        private System.Windows.Forms.Timer m_stopListeningTimer = null;
        private int m_tickTime = 60;
        private int m_originalVolume = 50;
        #endregion

        #region Constructor(s)
        public RiffSystemOperations()
        {
            m_greetings = Bootstrapper.ResolveType<Greetings>();
            m_speechSynthesizer = m_speechContext.SpeechSynthesizer;
            m_stopListeningTimer = new System.Windows.Forms.Timer();
            m_stopListeningTimer.Tick += new EventHandler(Tick);
            m_stopListeningTimer.Interval = 1000;
        }
        #endregion

        #region Public method(s)
        public void SetApplicationWindow(RiffApplication riffWindow)
        {
            m_riffWindow = riffWindow;
        }

        public override void HandleSpeechRequest(string speech)
        {
            if (speech.Contains("QUIET") || speech.Contains("SH") || speech.Contains("VOLUME DOWN"))
                JarvisVolume(true);
            else
            if (speech.Contains("LOUD") || speech.Contains("I CANT HEAR YOU") || speech.Contains("VOLUME UP"))
                JarvisVolume(false);
            else
            if (speech.Contains("MUTE"))
                JarvisMute(true);
            else 
            if (speech.Contains("UNMUTE") || speech.Contains("UNDO MUTE") || speech.Contains("SPEAK"))
                JarvisMute(false);
            else
            if (speech.Contains("STOP LISTENING"))
                StopListening();
            else
            if (speech.Contains("MINIMIZE") || speech.Contains("SMALL"))
                Minimize();
            else
            if (speech.Contains("MAXIMIZE") || speech.Contains("NORMAL") || speech.Contains("COME BACK"))
                Maximize();
            else
            if (speech.Contains("RIFF SLEEP") || speech.Contains("SLEEP") || speech.Contains("BYE"))
                EndProgram();
            else
                this.PassRequestHandling(speech);
        }

        public void Minimize()
        {
            m_riffWindow.WindowState = FormWindowState.Minimized;
            m_riffWindow.Icon = SystemIcons.Application;
            m_riffWindow.ShowInTaskbar = false;
            m_riffWindow.RiffNotifyIcon.BalloonTipTitle = "Riff";
            m_riffWindow.RiffNotifyIcon.BalloonTipText = "Running in background";
            m_riffWindow.RiffNotifyIcon.ShowBalloonTip(1500);
            m_riffWindow.RiffNotifyIcon.Visible = true;
            m_riffWindow.Hide();
        }

        public void Maximize()
        {
            m_riffWindow.WindowState = FormWindowState.Normal;
            m_riffWindow.Visible = false;
        }
        #endregion

        #region Private method(s)
        private void JarvisVolume(bool volumeDown)
        {
            int volume = m_speechSynthesizer.Volume;
            if (volumeDown)
            {
                if (m_speechSynthesizer.Volume == 0 || (m_speechSynthesizer.Volume - 20) < 0)
                {
                    m_speechSynthesizer.Volume = 20;
                    m_speechSynthesizer.Speak("Shhh...Muted");
                    m_speechSynthesizer.Volume = 0;
                }
                else
                {
                    m_speechSynthesizer.Volume -= 20;
                    m_speechSynthesizer.Speak("Getting quieter, m_volume decreased");
                }
            }
            else
            {
                if (m_speechSynthesizer.Volume == 100 || (m_speechSynthesizer.Volume + 20) > 100)
                {
                    m_speechSynthesizer.Speak("I am at my max m_volume, I would recommend not trying to go any louder");
                }
                else
                {
                    m_speechSynthesizer.Volume += 20;
                    m_speechSynthesizer.Speak("Yes, Volume increased");
                }
            }
        }

        private void JarvisMute(bool mute)
        {
            if (mute)
            {
                m_speechContext.Speak("Muting");
                m_originalVolume = m_speechSynthesizer.Volume;
                m_speechContext.SpeechSynthesizer.Volume = 0;
            }
            else
            {
                m_speechContext.SpeechSynthesizer.Volume = m_originalVolume;
                m_speechSynthesizer.Speak("Volume levels restored");
            }
        }

        private void StopListening()
        {
            m_speechContext.SpeechEngine.RecognizeAsyncStop();
            Console.WriteLine("Not Listening");
            m_stopListeningTimer.Start();
        }

        private void EndProgram()
        {
            var goodbyeThread = new Thread(new ThreadStart(m_greetings.Exit));
            goodbyeThread.IsBackground = true;
            goodbyeThread.Start();
            goodbyeThread.Join();

            m_speechSynthesizer.SpeakAsyncCancelAll();
            m_speechSynthesizer.Dispose();
            m_riffWindow.Close();
        }

        private void Tick(object sender, EventArgs e)
        {
            m_tickTime = m_tickTime - 1;
            Console.WriteLine(m_tickTime.ToString());
            if (m_tickTime == 0)
            {
                m_speechContext.SpeechEngine.RecognizeAsync(RecognizeMode.Multiple);
                Console.WriteLine("You may Speak");
                m_stopListeningTimer.Stop();
            }
        }
        #endregion
    }
}