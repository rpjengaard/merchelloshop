using System;
using System.Collections.Generic;
using code.Extensions;
using Newtonsoft.Json;
using Skybrud.Essentials.Enums;

namespace code.Models.Spa
{
    public class SpaRequestOptions {
        
        #region Properties

        [JsonProperty("id")]
        public int SiteId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

		[JsonProperty("isPreview")]
		public bool IsPreview { get; set; }

		[JsonProperty("parts")]
        public List<SpaApiPart> Parts { get; set; }

        public string Protocol { get; set; }

        public string HostName { get; set; }

        #endregion

        #region Constructors

        [JsonConstructor]
        public SpaRequestOptions()
        {
        }

        public SpaRequestOptions(int siteId)
        {
            SiteId = siteId;
            Url = "";
			IsPreview = false;
			Parts = GetParts();
        }

        public SpaRequestOptions(int siteId, string url)
        {
            SiteId = siteId;
            Url = url;
			IsPreview = url.IsPreviewUrl();
			Parts = GetParts();
        }

        public SpaRequestOptions(int siteId, string url, string parts)
        {
            SiteId = siteId;
			Url = string.IsNullOrWhiteSpace(url) ? "/" : url;
			IsPreview = Url.IsPreviewUrl();
			Parts = GetParts(parts);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Converts parts string[] to <see cref="List{SpaApiPart}"/>.
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        private static List<SpaApiPart> GetParts(string parts = "")
        {
            //if non return all
            if (String.IsNullOrWhiteSpace(parts))
                return new List<SpaApiPart> {SpaApiPart.Content, SpaApiPart.Navigation, SpaApiPart.Site};


            var partList = new List<SpaApiPart>();

            foreach (string part in parts.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                SpaApiPart e;
                if (EnumUtils.TryParseEnum(part, out e))
                {
                    partList.Add(e);
                }
            }

            return partList;
        }

        #endregion
    }

    public enum SpaApiPart
    {
        Content,
        Navigation,
        Site
    }
}