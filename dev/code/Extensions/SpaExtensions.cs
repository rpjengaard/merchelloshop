using System;
using System.Web;
using Skybrud.UmbracoEssentials.Extensions.PublishedContent;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace code.Extensions {

    public static class SpaExtensions {
        
        public static string GetSpaTitle(this IPublishedContent content) {
            if (!content.HasValue(Constants.SkyConstants.Properties.Title)) return content.Name;
            string title = content.GetString(Constants.SkyConstants.Properties.Title);
            return String.IsNullOrWhiteSpace(title) ? "" : HttpUtility.HtmlEncode(title).Replace("**", "<br />").Replace("*", "&shy;");
        }

        public static string GetSpaUrl(this IPublishedContent content) {

            string url = content.Url;

            if (content.DocumentTypeAlias == Constants.SkyConstants.DocumentTypes.FrontPage) {
                if (url.EndsWith("/" + content.UrlName + "/")) {
                    url = url.Substring(0, url.Length - content.UrlName.Length - 1);
                }
            }

            return url;

        }

        public static string GetSpaUrlWithDomain(this IPublishedContent content) {

            string url = content.UrlWithDomain();

            if (content.DocumentTypeAlias == Constants.SkyConstants.DocumentTypes.FrontPage) {
                if (url.EndsWith("/" + content.UrlName + "/")) {
                    url = url.Substring(0, url.Length - content.UrlName.Length - 1);
                }
            }

            return url;

        }

		/// <summary>
		/// Checks if node exists in UmbracoContext
		/// </summary>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		public static bool IsExistingNode(this int nodeId)
		{
			IPublishedContent check = UmbracoContext.Current.ContentCache.GetById(nodeId);

			return check != null;
		}

		public static bool IsPreviewUrl(this string url)
		{
			return url.GetPreviewId() > 0;
		}

		public static int GetPreviewId(this string url)
		{
			int nodeId = 0;

			if (url.Contains("/umbraco/dialogs"))
			{
				int.TryParse(url.Split('=')[1], out nodeId);
			}
			else
			{

				string[] urlFolders = url.Split('/');

				//tjek om der er indhold i arrayet
				if (urlFolders.Length <= 0) return -1;

				//find nodeId og returnér til nodeId var
				int.TryParse(urlFolders[1].Split('.')[0], out nodeId);
			}

			return nodeId;
		}
	}
}
