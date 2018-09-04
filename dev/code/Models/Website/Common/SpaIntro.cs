using Newtonsoft.Json;
using Skybrud.UmbracoEssentials.Extensions.PublishedContent;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace code.Models.Website.Common {

    public class SpaIntro {
        
        #region Properties

        [JsonProperty("teaser")]
        public string Teaser { get; }

		[JsonProperty("hasTeaser")]
		public bool HasTeaser { get { return !string.IsNullOrWhiteSpace(Teaser); } }

		[JsonProperty("image")]
        public SpaImage Image { get; }

        #endregion

        #region Constructors

        public SpaIntro(IPublishedContent content){
            Teaser = content.GetPropertyValue<string>(Constants.SkyConstants.Properties.Teaser);
            Image = content.TypedMedia(Constants.SkyConstants.Properties.Image, SpaImage.GetFromContent);
        }

        #endregion 

    }

}