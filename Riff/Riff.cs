﻿using System;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections.Generic;

namespace Riff
{
    public partial class RiffApplication : Form
    {
        #region Private Data
        private List<String> m_grammarPhrases = null;
        private RiffSystemOperations m_riffSystemOperations = null;
        private Greetings m_greetings = null;
        private SpeechHandlerChain m_speechHandlerChain = null;
        private SpeechContext m_speechContext = null;
        #endregion

        #region Constructor(s)
        public RiffApplication()
        {
            Initialize();
            LoadUpWelcome();
        }
        #endregion

        #region Private method(s)
        private void Initialize()
        {
            m_grammarPhrases = new List<String>();

            ResovleTypes();
            InitializeComponent();

            m_speechHandlerChain.SetupHandlerChain();

            m_riffSystemOperations.SetApplicationWindow(this);
            m_riffSystemOperations.Minimize();
            this.FormBorderStyle = FormBorderStyle.None;

            //Set up speech recognisition event handler
            m_speechContext.SpeechEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(RecognizeSpeech);
        }

        private void ResovleTypes()
        {
            m_riffSystemOperations = Bootstrapper.ResolveType<RiffSystemOperations>();
            m_greetings = Bootstrapper.ResolveType<Greetings>();
            m_speechHandlerChain = Bootstrapper.ResolveType<SpeechHandlerChain>();
            m_speechContext = Bootstrapper.ResolveType<SpeechContext>();
        }

        private void RecognizeSpeech(object sender, SpeechRecognizedEventArgs e)
        {
            var speech = e.Result.Text;
            Console.WriteLine(speech);
            m_speechHandlerChain.HandleSpeechRequest(speech);
        }
        #endregion

        #region Public method(s)
        public void LoadUpWelcome()
        {
            var welcomeThread = new Thread(new ThreadStart(m_greetings.Welcome));
            welcomeThread.IsBackground = true;
            welcomeThread.Start();
        }
        #endregion

        /* public googleOther google = new googleOther();
         public void googleSearch()
         {
             google.Show();

             Thread searchForThread = new Thread(new ThreadStart(() => m_speechContext.SearchFor()));
             searchForThread.IsBackground = true;
             searchForThread.Start();

             m_speechEngine.RecognizeAsyncCancel();

             m_speechEngine.SetInputToDefaultAudioDevice();
             m_speechEngine.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(getPhrases().ToArray()))));
             m_speechEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(whatToGoogle_SpeechRecognized);
             m_speechEngine.RecognizeAsync(RecognizeMode.Multiple);
         }

         private void whatToGoogle_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
         {
             if (e.Result.Text.ToUpper().Equals("SEARCH"))
             {
                 google.searchGoogle();

                 Thread loading = new Thread(new ThreadStart(() => m_speechContext.Loading()));
                 loading.IsBackground = true;
                 loading.Start();
                 try
                 {
                     m_speechEngine.RecognizeAsync(RecognizeMode.Multiple);
                 }
                 catch (System.Exception ex)
                 {
                     Console.WriteLine(ex.ToString());
                 }
                 m_speechEngine.RecognizeAsyncStop();
             }

         }*/
    }
}