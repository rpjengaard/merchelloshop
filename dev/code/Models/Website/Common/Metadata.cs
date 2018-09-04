using Newtonsoft.Json;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace code.Models.Website.Common
{
	public class MetaData
	{
		#region Properties
		[JsonProperty("title")]
		public string MetaTitle { get; set; }
		[JsonProperty("description")]
		public string MetaDescription { get; set; }
		[JsonProperty("noIndex")]
		public bool HideFromSearch { get; private set; }


		#endregion

		#region Constructors

		public MetaData(IPublishedContent content)
		{

			MetaTitle = content.HasValue(Constants.SkyConstants.Properties.SeoTitle) && !string.IsNullOrEmpty(content.GetPropertyValue<string>(Constants.SkyConstants.Properties.SeoTitle))
				? content.GetPropertyValue<string>(Constants.SkyConstants.Properties.SeoTitle)
				: (content.HasValue(Constants.SkyConstants.Properties.Title) && !string.IsNullOrEmpty(content.GetPropertyValue<string>(Constants.SkyConstants.Properties.Title))
					? content.GetPropertyValue<string>("title") : content.Name);

			MetaTitle = MetaTitle.Replace("*", "") + " - " + SkyContext.Current.Site.Content.GetPropertyValue<string>(Constants.SkyConstants.Properties.SiteName);

			MetaDescription = content.HasValue(Constants.SkyConstants.Properties.SeoMetaDescription) && !string.IsNullOrEmpty(content.GetPropertyValue<string>(Constants.SkyConstants.Properties.SeoMetaDescription))
				? content.GetPropertyValue<string>(Constants.SkyConstants.Properties.SeoMetaDescription)
				: (content.HasValue(Constants.SkyConstants.Properties.Teaser) && !string.IsNullOrEmpty(content.GetPropertyValue<string>(Constants.SkyConstants.Properties.Teaser))
					? content.GetPropertyValue<string>(Constants.SkyConstants.Properties.Teaser) : "");

			HideFromSearch = content.HasProperty(Constants.SkyConstants.Properties.HideFromSearch) && content.GetPropertyValue<bool>(Constants.SkyConstants.Properties.HideFromSearch);

		}

		#endregion

		#region Static methods

		public static MetaData GetFromContent(IPublishedContent content)
		{
			return new MetaData(content);
		}

		#endregion
	}
}
