using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace skybrudDk.Models.Website
{
    public class SkyMaster
    {
        #region Properties

        public IPublishedContent Content { get; private set; }

        public int Id
        {
            get { return Content.Id; }
        }

        public string Name
        {
            get { return Content.Name; }
        }

        public string Url
        {
            get { return Content.Url; }
        }

        public int SiteId
        {
            get { return Content.AncestorOrSelf(1).Id; }
        }

        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }

        public bool HideFromSearch { get; private set; }
        public bool HideFromNavigation { get; private set; }

        public string TimeSpend { get; set; }

        #endregion

        #region Constructors

        public SkyMaster(IPublishedContent content)
        {
            Content = content;
            MetaTitle = content.HasValue("seoTitle") && !string.IsNullOrEmpty(content.GetPropertyValue<string>("seoTitle"))
                ? content.GetPropertyValue<string>("seoTitle")
                : (content.HasValue("title") && !string.IsNullOrEmpty(content.GetPropertyValue<string>("title"))
                    ? content.GetPropertyValue<string>("title") 
                    : content.Name);

            MetaTitle = MetaTitle.Replace("*", "");
            MetaDescription = content.HasValue("seoMetaDescription") && !string.IsNullOrEmpty(content.GetPropertyValue<string>("seoMetaDescription"))
             ? content.GetPropertyValue<string>("seoMetaDescription")
             : (content.HasValue("teaser") && !string.IsNullOrEmpty(content.GetPropertyValue<string>("teaser"))
                ? content.GetPropertyValue<string>("teaser") 
                : "");

            HideFromNavigation = content.GetPropertyValue<bool>("umbracoNaviHide");
            HideFromSearch = content.HasProperty("hideFromSearch") && content.GetPropertyValue<bool>("hideFromSearch");
        }

        #endregion

        #region Member methods

        public bool IsDescendantOf(int parentId)
        {
            IPublishedContent scope = Content.Parent;
            while (scope != null)
            {
                if (scope.Id == parentId) return true;
                scope = scope.Parent;
            }
            return false;
        }

        public bool IsDescendantOf(IPublishedContent content)
        {
            return content != null && IsDescendantOf(content.Id);
        }

        public bool IsDescendantOf(SkyMaster content)
        {
            return content != null && IsDescendantOf(content.Id);
        }

        /// <summary>
        ///     Gets whether the instance is of type <code>T</code>.
        /// </summary>
        /// <typeparam name="T">The intended type.</typeparam>
        public bool IsOfType<T>() where T : SkyMaster
        {
            var casted = this as T;
            return casted != null;
        }

        /// <summary>
        ///     Gets whether the instance is of type <code>T</code>.
        /// </summary>
        /// <typeparam name="T">The intended type.</typeparam>
        /// <param name="value"></param>
        public bool IsOfType<T>(out T value) where T : SkyMaster
        {
            value = this as T;
            return value != null;
        }

        #endregion
    }
}
