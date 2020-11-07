using System.Windows.Forms;
using System.Threading;

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

            m_speechHandlerChain.SetupHandlerChain();

            m_riffSystemOperations.SetApplicationWindow(this);
            m_riffSystemOperations.Minimize();
            this.FormBorderStyle = FormBorderStyle.None;
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
            var speecRecognitionThread = new Thread(() =>
            {
                var result = m_speechRecognitionEngine.RecognizeSpeech().Result;
            });
            speecRecognitionThread.IsBackground = true;
            speecRecognitionThread.Start();
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
