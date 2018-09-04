using System;
using code.Models.Skybrud.Base;
using code.Models.Website.Common;
using Newtonsoft.Json;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace code.Models.Website.Search
{
	public class NewsSearchResult : BaseUmbracoItem
	{
		#region Properties
		[JsonProperty("newsdate")]
		public DateTime NewsDate { get; set; }
		[JsonProperty("intro")]
		public SpaIntro Intro { get; set; }

		#endregion


		#region Constructors

		private NewsSearchResult(IPublishedContent content) : base(content)
		{
			NewsDate = content.GetPropertyValue(Constants.SkyConstants.Properties.ContentDate, default(DateTime));

			Intro = new SpaIntro(content);
		}

		#endregion


		#region Static Members

		public static NewsSearchResult GetFromContent(IPublishedContent content)
		{
			return content == null ? null : new NewsSearchResult(content);
		}

		#endregion
	}
}
