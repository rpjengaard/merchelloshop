using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using code.Constants;
using code.Extensions;
using code.Models.Spa.Site;
using Newtonsoft.Json;
using Skybrud.Essentials.Strings;
using Skybrud.UmbracoEssentials.Extensions.PublishedContent;
using Umbraco.Core.Models;

namespace code.Models.Website.Common {

    public class SpaMetaData {

        #region Properties

        /// <summary>
        /// Gets the meta title of the currrent page.
        /// </summary>
        [JsonProperty("title")]
        public string MetaTitle { get; }

        /// <summary>
        /// Gets the meta description of the currrent page.
        /// </summary>
        [JsonProperty("description")]
        public string MetaDescription { get; }

        /// <summary>
        /// Gets the current page should be hidden in search results.
        /// </summary>
        [JsonIgnore]
        public bool HideFromSearch { get; }

        /// <summary>
        /// Gets the robots value for the current page.
        /// </summary>
        [JsonProperty("robots")]
        public string Robots { get; }

        /// <summary>
        /// Gets the Open Graph title for the current page.
        /// </summary>
        [JsonProperty("og:title")]
        public string OpenGraphTitle { get; }

        /// <summary>
        /// Gets the Open Graph description for the current page.
        /// </summary>
        [JsonProperty("og:description")]
        public string OpenGraphDescription { get; }

        /// <summary>
        /// Gets the Open Graph site name.
        /// </summary>
        [JsonProperty("og:site_name")]
        public string OpenGraphSiteName { get; }

        /// <summary>
        /// Gets the Open Graph URL for the current page.
        /// </summary>
        [JsonProperty("og:url")]
        public string OpenGraphUrl { get; }

        /// <summary>
        /// Gets a collection of Open Graph images for the current page.
        /// </summary>
        [JsonProperty("og:image")]
        public List<SpaOpenGraphImage> OpenGraphImages { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new meta data object from the specified <paramref name="site"/> and <paramref name="content"/>.
        /// </summary>
        /// <param name="site">A reference to the current site.</param>
        /// <param name="content">A reference the content item representing the current page.</param>
        public SpaMetaData(SpaSiteModel site, IPublishedContent content) {
            
            string seoTitle = content.GetString(Constants.SkyConstants.Properties.SeoTitle);
            string seoMetaDescription = content.GetString(Constants.SkyConstants.Properties.SeoMetaDescription);

            string contentName = content.Name;
            string contentTitleText = content.GetSpaTitle().Replace("&shy;", "").Replace("<br />", "");
            string contentTeaserText = StringUtils.StripHtml(content.GetString(Constants.SkyConstants.Properties.Teaser) ?? "");

            MetaTitle = GetString(seoTitle, contentTitleText, contentName) + " - " + site.Name;
            MetaDescription = GetString(seoMetaDescription, contentTeaserText);
            
            // Should the page be hidden from search engines?
            if (HttpContext.Current.Request.Url.Host.Contains(SkyConstants.SolutionId + ".")) {
                HideFromSearch = true;
                Robots = "noindex, nofollow";
            } else if (content.GetBoolean("hideFromSearch")) {
                HideFromSearch = true;
                Robots = "noindex, follow";
            } else {
                Robots = "index, follow";
            }

            string siteUrl = site.Content.GetSpaUrlWithDomain();

            string ogTitle = content.GetString(SkyConstants.Properties.OpenGraphTitle);
            string ogDescription = content.GetString(SkyConstants.Properties.OpenGraphDescription);
            SpaImage ogImage = content.TypedMedia(SkyConstants.Properties.OpenGraphImage, SpaImage.GetFromContent) ?? content.TypedMedia(Constants.SkyConstants.Properties.Image, SpaImage.GetFromContent);
            
            OpenGraphTitle = GetString(ogTitle, contentTitleText, contentName);
            OpenGraphDescription = GetString(ogDescription, seoMetaDescription, contentTeaserText);
            OpenGraphSiteName = site.Name;
            OpenGraphUrl = content.GetSpaUrl();

            OpenGraphImages = new List<SpaOpenGraphImage>();

            if (ogImage != null) {
                if (ogImage.Width >= 1200 && ogImage.Height >= 630) {
                    string cropUrl = siteUrl + ogImage.GetCropUrl(1200, 630).TrimStart('/');
                    OpenGraphImages.Add(new SpaOpenGraphImage(cropUrl, 1200, 630));
                } else if (ogImage.Width >= 600 && ogImage.Height >= 315) {
                    string cropUrl = siteUrl + ogImage.GetCropUrl(600, 315);
                    OpenGraphImages.Add(new SpaOpenGraphImage(cropUrl, 600, 315));
                }
            }

        }

        #endregion

        #region Static methods

        private static string GetString(params string[] values) {
            return values.FirstOrDefault(x => !String.IsNullOrWhiteSpace(x)) ?? "";
        }

        /// <summary>
        /// Initializes a new meta data object from the specified <paramref name="site"/> and <paramref name="content"/>.
        /// </summary>
        /// <param name="site">A reference to the current site.</param>
        /// <param name="content">A reference the content item representing the current page.</param>
        /// <returns>An instance of <see cref="SpaMetaData"/>.</returns>
        public static SpaMetaData GetFromContent(SpaSiteModel site, IPublishedContent content) {
            return new SpaMetaData(site, content);
        }

        #endregion

    }

}