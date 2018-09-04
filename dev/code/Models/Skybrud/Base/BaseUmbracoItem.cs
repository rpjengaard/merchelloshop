using System;
using System.Linq;
using Newtonsoft.Json;
using Umbraco.Core.Models;

namespace code.Models.Skybrud.Base
{
    public class BaseUmbracoItem
    {
        #region Properties

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("path")]
        public int[] Path { get; set; }

        #endregion

        #region Constructors

        public BaseUmbracoItem(IPublishedContent content)
        {
            Id = content.Id;
            Name = content.Name;
            Url = content.Url;
            Path = content.Path.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
        }

        #endregion
    }
}
