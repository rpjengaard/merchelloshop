using System.Collections.Generic;
using code.Constants;
using code.Models.Spa.Navigation;
using Newtonsoft.Json;
using Skybrud.UmbracoEssentials.Extensions.PublishedContent;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace code.Models.Spa.Site {

    public class SpaSiteModel {

        private IEnumerable<NavItem> _mainNavigation;

        private SpaSiteFooter _footer;

        #region Properties

        /// <summary>
        /// Gets a reference to the root content node.
        /// </summary>
        [JsonIgnore]
        public IPublishedContent Content { get; }

        /// <summary>
        /// Gets a reference to the content node that provides culture specific settings for the site.
        /// </summary>
        [JsonIgnore]
        public IPublishedContent Culture { get; }

        /// <summary>
        /// Gets the name of the site.
        /// </summary>
        [JsonProperty("siteName")]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the ID of the site.
        /// </summary>
        [JsonProperty("siteRootId")]
        public int Id { get; private set; }

        /// <summary>
        /// Gets a reference to the main navigation.
        /// </summary>
        [JsonProperty("mainNavigation")]
        public IEnumerable<NavItem> MainNavigation => GetMainNavigation();

        /// <summary>
        /// Gets a reference to the footer.
        /// </summary>
        [JsonProperty("footer")]
        public SpaSiteFooter Footer => _footer ?? (_footer = new SpaSiteFooter(this));

        #endregion
        
        #region Constructors

        public SpaSiteModel(IPublishedContent site, IPublishedContent culture) {

            // Site
            Content = site;
            Id = Content.Id;

            // Culture
            Culture = culture;
            Name = culture.GetString(SkyConstants.Properties.SiteName);

        }

        #endregion

        #region Member methods

        /// <summary>
        /// Gets a reference to the main navigation. The property uses lazy loading, meaning that it's value isn't
        /// populated by the constructor, buit instead the first time the property is requested.
        /// </summary>
        /// <returns>An instance of <see cref="IEnumerable{NavItem}"/>.</returns>
        protected virtual IEnumerable<NavItem> GetMainNavigation() {

            // Just return the value if already loaded
            if (_mainNavigation != null) return _mainNavigation;

            // Get the main nav items from the "culture" node
            IEnumerable<IPublishedContent> items = Culture.GetPropertyValue<IEnumerable<IPublishedContent>>(SkyConstants.Properties.MainNavigation);

            // Map the nav items and set the private field
            _mainNavigation = _mainNavigation = NavItem.GetItems(items);

            // Return the value
            return _mainNavigation;

        }

        /// <summary>
        /// Gets a reference to the site footer. The property uses lazy loading, meaning that it's value isn't
        /// populated by the constructor, buit instead the first time the property is requested.
        /// </summary>
        /// <returns>An instance of <see cref="SpaSiteFooter"/>.</returns>
        protected virtual SpaSiteFooter GetFooter() {
            return _footer ?? (_footer = new SpaSiteFooter(this));
        }

        #endregion

    }

}