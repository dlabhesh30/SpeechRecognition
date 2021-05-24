using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.Components.Google
{
    public enum MetaType
    {
        Website, 
        Article,
        Video, 
        Unknown
    }

    internal class GoogleSearchResponseModel
    {
        #region Contructor(s)
        public GoogleSearchResponseModel()
        {
        }
        #endregion

        #region Private Method(s)
        private MetaType GetMetaType(string metaType)
        {
            var type = MetaType.Unknown;
            switch (metaType)
            {
                case "article":
                    type = MetaType.Article;
                    break;
                case "website":
                    type = MetaType.Website;
                    break;
                case "video.other":
                    type = MetaType.Video;
                    break;
            }
            return type;
        }

        private void SetSearchTerm(JObject searchObject)
        {
            var queryRequest = (JArray)searchObject["queries"]["request"];
            foreach(var request in queryRequest)
            {
                var searchTerm = (string)request["searchTerms"];
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    SearchTerm = searchTerm;
                    return;
                }
            }
            
        }
        #endregion

        #region Public Method(s)
        public bool SetSearchResponseModel(string searchResponse)
        {
            var result = false;
            try
            {
                var searchResponseObject = JObject.Parse(searchResponse);

                SetSearchTerm(searchResponseObject);

                TotalResults = (double)searchResponseObject["searchInformation"]["totalResults"];

                var searchItems = (JArray)searchResponseObject["items"];
                CurrentPageItems = new List<Item>();

                foreach (JObject searchItem in searchItems)
                {
                    //Get the first meta tag object
                    var metatag = (JObject)searchItem["pagemap"]["metatags"].First();
                    MetaInfo metaInfo = null;
                    if (metatag.Count > 0)
                    {
                        metaInfo = new MetaInfo()
                        {
                            ImageURL = (string)metatag["og:image"],
                            Type = GetMetaType((string)metatag["og:type"]),
                            SiteName = (string)metatag["og:site_name"]
                        };
                    }
                    
                    var item = new Item()
                    {
                        Title = (string)searchItem["title"],
                        Link = (string)searchItem["link"],
                        Meta = metaInfo
                    };
                    CurrentPageItems.Add(item);
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
        public List<Item> CurrentPageItems
        {
            get;
            set;
        }

        public string SearchTerm
        {
            get;
            set;
        }

        public double TotalResults
        {
            get;
            set;
        }
        #endregion
    }

    internal class Item
    {
        #region Public Properties
        public string Title
        {
            get;
            set;
        }

        public string Link
        {
            get;
            set;
        }

        public MetaInfo Meta
        {
            get;
            set;
        }
        #endregion
    }

    internal class MetaInfo
    {
        #region Public Properties
        public string ImageURL
        {
            get;
            set;
        }

        public MetaType Type
        {
            get;
            set;
        }

        public string SiteName
        {
            get;
            set;
        }
        #endregion
    }
}
