using code.Models.Spa.Site;
using Newtonsoft.Json;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace code.Models.Website.Pages {

    public class FrontPageModel : Master {
        
        #region Properties

        [JsonProperty("test")]
	    public string Test { get; private set; }

        #endregion

        #region Constructors

        public FrontPageModel(SpaSiteModel site, IPublishedContent content) : base(site, content) {
		    Test = content.GetPropertyValue<string>("test");
		}

        #endregion

        #region Static methods

        public static FrontPageModel GetFromContent(SpaSiteModel site, IPublishedContent content) {
            return content == null ? null : new FrontPageModel(site, content);
        }

        #endregion

    }

}