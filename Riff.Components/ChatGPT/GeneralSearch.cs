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
    enum GPTModel
    { 
        TextDaVinci003
    }

    internal class ChatGptSearchRequestBody
    {
        #region Private properties
        private string m_gptModel { get; set; }
        private string m_searchContent { get; set; }
        private double m_temperature { get; set; }
        private int m_maxTokens { get; set; }
        private double m_topP { get; set; }
        private double m_frequencyPenalty { get; set; }
        private double m_presencePenalty { get; set; }

        private Dictionary<GPTModel, string> m_GPTModelMapping;
        #endregion

        #region Constructor(s)
        public ChatGptSearchRequestBody(string searchContent, GPTModel gptModel = GPTModel.TextDaVinci003)
        { 
            PopulateGPTModelMapping();
            PopulateDefaultValuesForSearchRequest();
            SetSearchRequestDetails(searchContent, gptModel);
        }
        #endregion

        #region Private method(s)
        private void PopulateGPTModelMapping()
        {
            m_GPTModelMapping = new Dictionary<GPTModel, string>
            {
                { GPTModel.TextDaVinci003, "text-davinci-003" }
            };
        }

        private void PopulateDefaultValuesForSearchRequest() 
        { 
            m_frequencyPenalty = 0.5;
            m_presencePenalty = 0.0;
            m_topP = 0;
            m_maxTokens = 60;
            m_temperature = 0.5;
            m_gptModel = m_GPTModelMapping[GPTModel.TextDaVinci003];
        }

        private void SetSearchRequestDetails(string searchContent, GPTModel gptModel = GPTModel.TextDaVinci003)
        {
            m_gptModel = m_GPTModelMapping[gptModel];
            m_searchContent = searchContent;
        }
        #endregion

        #region Public Method(s)
        public object SearchRequestBody() 
        {
            var requestBody = new
            {
                model = m_gptModel,
                prompt = m_searchContent,
                temperature = m_temperature,
                max_tokens = m_maxTokens,
                top_p = m_topP,
                frequency_penalty = m_frequencyPenalty,
                presence_penalty = m_presencePenalty
            };
            return requestBody;
        }
        #endregion
    }


    public class GeneralSearch : AbstractSpeechHandler
    {
        #region Private Data
        private string m_apiBasePath = "https://api.openai.com/v1/completions";
        private WebRequest m_webRequest = null;
        private GeneralSearchResponseModel m_searchResponseModel = null;
        private string m_apiBearerAuth = "";
        private List<string> m_searchTerms = new List<string>
        {
            "SEARCH",
            "SEARCH WEB",
            "SEARCH ONLINE",
            "TELL ME ABOUT",
            "GIVE"
        };
        #endregion

        #region Constructor(s)
        public GeneralSearch(ISpeechContext speechContext, WebRequest webRequest)
            : base(speechContext)
        {
            m_webRequest = webRequest;
            m_searchResponseModel = new GeneralSearchResponseModel();
        }
        #endregion

        #region Public method(s)
        public override void HandleSpeechRequest(string speech)
        {
            if (m_searchTerms.Any(s => speech.StartsWith(s, StringComparison.OrdinalIgnoreCase) || speech.Contains(s)))
            {
                QueryChatGPT(speech);
            }
            else
            {
                PassRequestHandling(speech);
            }

        }
        #endregion

        #region Private method(s)
        private string RequestUrl()
        {
            return m_apiBasePath;
        }

        private string FormatSearchTerm(string queryString)
        {
            var result = string.Empty;
            foreach (var searchTerm in m_searchTerms)
            {
                result = queryString.Replace(searchTerm, string.Empty);
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
            var searchTerm = FormatSearchTerm(queryString);
            ChatGptSearchRequestBody requestBody = new ChatGptSearchRequestBody(searchTerm);
            var responseString = m_webRequest.PostRequest(RequestUrl(), m_apiBearerAuth, requestBody.SearchRequestBody()).Result;
            if (SetSearchResponseModel(responseString))
            {
                SpeakSearchResults();
            }
        }

        private bool SetSearchResponseModel(string response)
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
            var searchSpeechThread = new Thread(new ThreadStart(() => m_speechContext.Speak(m_searchResponseModel.SearchResponse)));
            searchSpeechThread.IsBackground = true;
            searchSpeechThread.Start();
        }
        #endregion
    }
}
