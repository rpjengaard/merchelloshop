using System;
using System.Web;
using code.Models.Common;
using umbraco.cms.businesslogic.web;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace code
{
    public class SkyContext
    {
        #region Private properties

        private SkySite _site;
        private SkyContext()
        {
            Repositories = new Repositories();
        }
        #endregion

        #region Puplic properties

        public Repositories Repositories { get; private set; }

        public static SkyContext Current
        {
            get
            {
                if (HttpContext.Current == null) return new SkyContext();

                var context = HttpContext.Current.Items["SkyContext:Current"] as SkyContext;

                if (context == null)
                {
                    HttpContext.Current.Items["SkyContext:Current"] = context = new SkyContext();
                }

                return context;
            }
        }

        /// <summary>
        /// Gets a reference to the current <code>PublishedContentRequest</code>.
        /// </summary>
        public PublishedContentRequest Request
        {
            get { return UmbracoContext.Current.PublishedContentRequest; }
        }

        /// <summary>
        /// Gets a reference to the current <code>Settings</code>.
        /// </summary>
        public SkySite Site
        {
            get { return _site ?? (_site = FindSiteNode()); }
        }

        #endregion

        #region Private methods

        private SkySite FindSiteNode()
        {
            PublishedContentRequest request = UmbracoContext.Current.PublishedContentRequest;
            // Get a reference to the current request
            IPublishedContent root;
            // Attempt to find the site node by using an ancestor lookup (should work for non-virtual pages)
            if (request == null)
            {
                var domain = Domain.GetRootFromDomain(HttpContext.Current.Request.Url.Host);
                if (domain == 0)
                {
                    throw new Exception("domain not found : " + domain);
                }
                else
                {
                    root = UmbracoContext.Current.ContentCache.GetById(domain);
                }

            }
            else
            {
                root = Request.InitialPublishedContent != null ?
                    Request.InitialPublishedContent.AncestorOrSelf(Constants.SkyConstants.DocumentTypes.Site) :
                    Request.PublishedContent.AncestorOrSelf(Constants.SkyConstants.DocumentTypes.Site);

            }

            // Attempt to find the root node based on the domain if not already found at this point
            if (root == null && Request.HasDomain)
            {
                root = UmbracoContext.Current.ContentCache.GetById(Request.Domain.RootNodeId);
            }

            return SkySite.GetFromContent(root);
        }

        #endregion
    }
}
