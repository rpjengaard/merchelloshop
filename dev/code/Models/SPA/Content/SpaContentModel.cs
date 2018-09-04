using code.Constants;
using code.Models.Spa.Site;
using code.Models.Website;
using code.Models.Website.Pages;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace code.Models.Spa.Content {

    public class SpaContentModel {
        
        #region Properties

        public string AppHost { get; }

        public string AppProtocol { get; }

        public int SiteId { get; }

        #endregion

        #region Constructors

        public SpaContentModel(SpaSiteModel site, string appHost, string appProtocol) {
            AppHost = appHost;
            AppProtocol = appProtocol;
            SiteId = site.Id;
        }

        #endregion

        #region Public

        public Master GetContent(SpaSiteModel site, IPublishedContent content) {

            if (content.HasProperty(SkyConstants.Properties.UmbracoInternalRedirect) && content.HasValue(SkyConstants.Properties.UmbracoInternalRedirect)) {
				content = new UmbracoHelper(UmbracoContext.Current).TypedContent(content.GetPropertyValue<Udi>(SkyConstants.Properties.UmbracoInternalRedirect));
			}
            
			switch (content.DocumentTypeAlias) {

                case SkyConstants.DocumentTypes.FrontPage:
                    return FrontPageModel.GetFromContent(site, content);

                case SkyConstants.DocumentTypes.SubPage:
                    return SubPageModel.GetFromContent(site, content);

                default:
                    return null;
            }

        }

        #endregion

    }

}