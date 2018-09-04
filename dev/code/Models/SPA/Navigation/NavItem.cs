using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using code.Extensions;
using Newtonsoft.Json;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace code.Models.Spa.Navigation{
    public class NavItem {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("parentId")]
        public int ParentId { get; set; }

        [JsonProperty("template")]
        public string Template { get; set; }

        [JsonProperty("culture")]
        public string Culture { get; set; }

        [JsonProperty("hasChildren")]
        public bool HasChildren { get; set; }

        [JsonProperty("isVisible")]
        public bool IsVisible { get; set; }

        [JsonProperty("children")]
        public NavItem[] Children { get; set; }


        public static NavItem GetItem(IPublishedContent content, int levels = 1, int levelcount = 1)
        {
            if (content == null) return null;

            IPublishedContent[] children = content.Children(x => x.TemplateId > 0 && x.IsVisible()).ToArray();

            CultureInfo culture = content.GetCulture();

            return new NavItem{
                Id = content.Id,
                Title = content.GetSpaTitle(),
                Url = content.Url,
                ParentId = content.Parent != null ? content.Parent.Id : -1,
                Template = content.GetTemplateAlias(),
                Culture = culture.Name,
                HasChildren = children.Any(),
                IsVisible = !content.HasProperty(Constants.SkyConstants.Properties.UmbracoNaviHide) || !content.GetPropertyValue<bool>(Constants.SkyConstants.Properties.UmbracoNaviHide),
                Children = children.Any() && levels > levelcount ? children.Select(x => NavItem.GetItem(x, levels)).ToArray() : new NavItem[0],
            };
        }

        
        public static IEnumerable<NavItem> GetItems(IEnumerable<IPublishedContent> content, int levels = 1, int levelcount = 1){
            if (content == null) return null;

            LogHelper.Info<NavItem>("Count: " + content.Count());

            levelcount++;

            return content.Select(x => GetItem(x, levels, levelcount)).Where(x => x.IsVisible);
        }

        public static IEnumerable<NavItem> GetItems(int[] ids, int levels = 1, int levelcount = 1){
            if (ids.Length == 0) return null;

            return ids.Select(x => UmbracoContext.Current.ContentCache.GetById(x)).Where(x => x != null).Select(x => GetItem(x, levels, levelcount)).Where(x => x.IsVisible);
        }
    }
}