using System;
using System.Collections.Generic;
using System.Linq;
using code.Extensions;
using Newtonsoft.Json;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace code.Models.Spa.Navigation
{
    public class SpaNavigationModel
    {
        #region Properties

        [JsonProperty("children")]
        public List<NavItem> Children { get; set; }

        [JsonProperty("context")]
        public NavItem Context { get; set; }

        #endregion


        #region Constructors

        [JsonConstructor]
        public SpaNavigationModel()
        {
        }

        public SpaNavigationModel(IPublishedContent content, int levels, bool navContext = false)
        {
            if (content == null) return;

            List<IPublishedContent> path = new List<IPublishedContent>();

            // fill path
            if (navContext)
            {
                string[] _path = content.Path.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                path = _path.Where(x => int.Parse(x) > 0).Select(x => UmbracoContext.Current.ContentCache.GetById(int.Parse(x))).Where(x => x.IsVisible()).ToList();

                // context
                Context = GetNavContext(path);
            }

            // children
            Children = content.Children().Where(x => x.IsVisible()).Select(x => NavItem.GetItem(x, levels)).ToList();
        }

        #endregion


        #region Private

        private NavItem GetNavContext(IEnumerable<IPublishedContent> path)
        {
            if (path == null) return null;

            // first level node
            IPublishedContent rootNode = path.FirstOrDefault();

            // find children
            var levelOneChildren = rootNode.Children.Where(x => x.IsVisible());

            NavItem root = new NavItem()
            {
                Id = rootNode.Id,
                Title = rootNode.GetSpaTitle(),
                Url = rootNode.Url,
                ParentId = rootNode.Parent != null ? rootNode.Parent.Id : -1,
                Template = rootNode.GetTemplateAlias(),
                Culture = rootNode.GetCulture().Name,
                HasChildren = levelOneChildren.Any(),
                IsVisible = rootNode.IsVisible(),
                Children = AddNavItemsFromPath(rootNode, path)
            };

            return root;
        }


        private NavItem[] AddNavItemsFromPath(IPublishedContent content, IEnumerable<IPublishedContent> path)
        {
            if (content == null) return null;
            if (path == null) return null;

            IEnumerable<IPublishedContent> children = content.Children.Where(x => x.IsVisible());

            var o = children.Any(path.Contains)
                ? children.Select(y => new NavItem()
                {
                    Id = y.Id,
                    Title = y.Name,
                    Url = y.Url,
                    ParentId = y.Parent != null ? y.Parent.Id : -1,
                    Template = y.GetTemplateAlias(),
                    Culture = y.GetCulture().Name,
                    HasChildren = y.Children.Any(x => x.IsVisible()),
                    IsVisible = y.IsVisible(),
                    Children = AddNavItemsFromPath(y, path)
                }).ToArray()
                : null;

            return o;
        }

        #endregion
    }
}
