using code.Models.Spa.Site;
using Umbraco.Core.Models;

namespace code.Models.Website.Pages {

	public class SubPageModel : Master {

        #region Properties

        #endregion
        
        #region Constructors

        public SubPageModel(SpaSiteModel site, IPublishedContent content) : base(site, content) {

        }

        #endregion

        #region Static methods

        public static SubPageModel GetFromContent(SpaSiteModel site, IPublishedContent content) {
            return content == null ? null : new SubPageModel(site, content);
        }

        #endregion

    }

}