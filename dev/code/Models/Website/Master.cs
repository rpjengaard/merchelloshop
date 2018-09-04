using System;
using System.Linq;
using code.Extensions;
using code.Models.Spa.Site;
using code.Models.Website.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace code.Models.Website {

    public class Master {
        
        #region Properties

        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("url")]
        public string Url { get; private set; }

        [JsonProperty("title")]
        public string Title { get; private set; }

        [JsonProperty("intro")]
        public SpaIntro Intro { get; }

        [JsonProperty("noCache")]
        public bool NoCache { get; set; }

        [JsonProperty("meta")]
        public SpaMetaData Meta { get; }

        [JsonProperty("path")]
        public int[] Path { get; private set; }

        [JsonProperty("template")]
        public string Template { get; private set; }

        [JsonProperty("culture")]
        public string Culture { get; private set; }

        [JsonProperty("created")]
        public DateTime Created { get; private set; }

        [JsonProperty("updated")]
        public DateTime Updated { get; private set; }

        [JsonProperty("jsonDebug", NullValueHandling = NullValueHandling.Ignore)]
        public JObject JsonDebug { get; private set; }

        #endregion
        
        #region Constructors

        public Master(SpaSiteModel site, IPublishedContent content){

            Id = content.Id;

            Title = content.GetSpaTitle();

            Url = content.GetSpaUrl();

            Meta = SpaMetaData.GetFromContent(site, content);
            Intro = new SpaIntro(content);

            Path = content.Path.Split(',').Select(x => Convert.ToInt32(x)).Skip(1).ToArray();
            Template = content.GetTemplateAlias();
	        Culture = content.GetCulture().Name;
            Created = content.CreateDate;
            Updated = content.UpdateDate;
            NoCache = content.GetPropertyValue<bool>(Constants.SkyConstants.Properties.NoCache);
            JsonDebug = content.HasProperty(Constants.SkyConstants.Properties.JsonData) && content.HasValue(Constants.SkyConstants.Properties.JsonData) ? JObject.Parse(content.GetPropertyValue<string>(Constants.SkyConstants.Properties.JsonData).Remove(0, 1)) : null;

        }

        #endregion 


        public bool IsOfType<T>() where T : Master
        {
            T casted = this as T;
            return casted != null;
        }

        public bool IsOfType<T>(out T value) where T : Master
        {
            value = this as T;
            return value != null;
        }
    }
}
