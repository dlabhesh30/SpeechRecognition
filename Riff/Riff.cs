using System;
using System.Windows.Forms;
using System.Threading;

using Riff.Components;
using Riff.Framework;

namespace Riff
{
    public partial class RiffApplication : Form
    {
        #region Private Data
        private RiffSystemOperations m_riffSystemOperations = null;
        private Greetings m_greetings = null;
        private ISpeechHandlerChain m_speechHandlerChain = null;
        private ISpeechContext m_speechContext = null;
        private IRecognitionEngineProvider m_speechRecognitionEngine = null;
        #endregion

        #region Constructor(s)
        public RiffApplication()
        {
            Initialize();
            LoadUpWelcome();
            StartRecognizing();
        }
        #endregion

        #region Private method(s)
        private void Initialize()
        {
            ResovleTypes();
            InitializeComponent();

            m_riffSystemOperations.StopListeningEvent += OnStopListeningEvent;
            m_speechHandlerChain.SetupHandlerChain();

            m_riffSystemOperations.SetApplicationWindow(this);
            m_riffSystemOperations.Minimize();
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void OnStopListeningEvent(object sender, EventArgs e)
        {
            m_speechRecognitionEngine.StopListening();
        }

        private void ResovleTypes()
        {
            m_riffSystemOperations = Bootstrapper.ResolveType<RiffSystemOperations>();
            m_greetings = Bootstrapper.ResolveType<Greetings>();
            m_speechHandlerChain = Bootstrapper.ResolveType<ISpeechHandlerChain>();
            m_speechContext = Bootstrapper.ResolveType<ISpeechContext>();
            m_speechRecognitionEngine = Bootstrapper.ResolveType<IRecognitionEngineProvider>();
        }

        private void StartRecognizing()
        {
            m_speechRecognitionEngine.RecognizeSpeech();
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
    }
}
