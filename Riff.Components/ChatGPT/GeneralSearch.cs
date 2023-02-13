using Riff.Components.Google;
using Riff.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Riff.Components.ChatGPT
{
    public class GeneralSearch : AbstractSpeechHandler
    {
        #region Private Data
        private string m_apiBasePath = "https://api.openai.com/v1/completions";
        private WebRequest m_webRequest = null;
        private GoogleSearchResponseModel m_searchResponseModel = null;
        private string m_apiBearerAuth = "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";
        private string m_searchEngineIdParam = "cx=";
        #endregion

        #region Constructor(s)
        public GeneralSearch(ISpeechContext speechContext, WebRequest webRequest)
            : base(speechContext)
        {
            m_webRequest = webRequest;
            m_searchResponseModel = new GoogleSearchResponseModel();
        }
        #endregion

        #region Public method(s)
        public override void HandleSpeechRequest(string speech)
        {
            if (speech.Contains("SEARCH") || speech.Contains("SEARCH ONLINE") || speech.Contains("SEARCH WEB"))
                QueryChatGPT(speech);
            else
                this.PassRequestHandling(speech);

        }
        #endregion

        #region Private method(s)
        private string RequestUrl(string queryString)
        {
            return m_apiBasePath;
        }

        private string FormatQueryString(string queryString)
        {
            var result = "";
            var querySplit = queryString.Split(new[] { "FOR" }, StringSplitOptions.None);
            if (querySplit.Length > 1)
            {
                result = querySplit[1];
            }
            return result;
        }

        private void QueryChatGPT(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
            {
                m_speechContext.Speak("Empty search query ! Please tell me what to search.");
                return;
            }
            var query = FormatQueryString(queryString);
            var responseString = m_webRequest.PostRequest(RequestUrl(query), m_apiBearerAuth).Result;
            if (SetGoogleResponseModel(responseString))
            {
                SpeakSearchResults();
            }
        }

        private bool SetGoogleResponseModel(string response)
        {
            var result = false;
            if (!string.IsNullOrEmpty(response) && IsResponseValid(response))
            {
                result = m_searchResponseModel.SetSearchResponseModel(response);
            }
            return result;
        }

        private bool IsResponseValid(string response)
        {
            var result = true;

            return result;
        }

        private void SpeakSearchResults()
        {
            /*var weather = new StringBuilder();
            weather.AppendLine("I see " + weatherModel.Description);
            weather.AppendLine("It feels like " + weatherModel.FeelsLike + " degree celsius.");
            weather.AppendLine("and Humidity is " + weatherModel.Humidity);
            */
            if (m_searchResponseModel.TotalResults > 0)
            {
                var searchSpeechThread = new Thread(new ThreadStart(() => m_speechContext.Speak(m_searchResponseModel.SearchTerm)));
                searchSpeechThread.IsBackground = true;
                searchSpeechThread.Start();
            }
        }
        #endregion
    }
}
