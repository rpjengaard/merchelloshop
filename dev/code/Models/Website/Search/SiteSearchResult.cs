using code.Models.Skybrud.Base;
using Newtonsoft.Json;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace code.Models.Website.Search
{
    public class SiteSearchResult : BaseUmbracoItem
    {
		#region Properties

		[JsonProperty("teaser")]
		public string Teaser { get; private set; }

		[JsonProperty("type")]
		public string Type { get; private set; }

		#endregion


		#region Constructors

		public SiteSearchResult(IPublishedContent content) : base(content)
		{
			Teaser = content.HasValue(Constants.SkyConstants.Properties.Teaser) ? content.GetPropertyValue<string>(Constants.SkyConstants.Properties.Teaser) : null;
			Type = "page";
		}

		#endregion


		#region Static methods

		public static SiteSearchResult GetFromContent(IPublishedContent content)
		{
			return content == null ? null : new SiteSearchResult(content);
		}
		#endregion

	}
}
