using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;

namespace Riff.Components.Google
{
    internal class GeneralSearchResponseModel
    {
        #region Public Method(s)
        public bool SetSearchResponseModel(string searchResponse)
        {
            var result = false;
            try
            {
                var responseObject = JObject.Parse(searchResponse);
                var error = (JObject)responseObject["error"];
                if (error != null)
                {
                    SearchResponse = string.Format("Error searching: {0}", error["message"].ToString());
                }
                else
                {
                    var searchItems = (JArray)responseObject["choices"];
                    if (searchItems.Count > 0)
                    { 
                        foreach (JObject searchItem in searchItems)
                        {
                            var responseTextObject = searchItem["text"];
                            if (responseTextObject != null)
                            {
                                SearchResponse = responseTextObject.ToString();
                            }
                        }
                    }
                    else
                    {
                        SearchResponse = "Got no results for the search";
                    }
                }
                result = true;
            }
            catch (Exception e)
            {
                //TODO: Think about bubbling this upto the higher up GooglSearch class
                Console.WriteLine(e.ToString());
                return result;
            }
            return result;
        }

        #endregion

        #region Public Properties
        public string SearchResponse
        {
            get;
            set;
        }
        #endregion
    }
}
