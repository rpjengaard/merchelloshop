using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Skybrud.Umbraco.Module.Extensions.PublishedContent;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace code.Extensions
{
    public static class Extensions
    {
		public static string UrlReferrer(this HttpRequestMessage request)
		{
			return request.Headers.Referrer == null ? "unknown" : request.Headers.Referrer.AbsoluteUri;
		}

		/// <summary>
		/// Konverterer en tekst med linieskift til html (<br /> eller <p>)
		/// </summary>
		/// <param name="text">Strengen der skal konverteres</param>
		/// <param name="cssClass">Hvis udfyldt, sættes dette klassenavn rundt om teksten i et p-tag</param>
		/// <returns></returns>
		public static MvcHtmlString Nl2P(this string text, string cssClass = "")
        {
            text = text ?? "";

            text = text.Trim().Replace("\r", "");

            var start = cssClass == "" ? "<p>" : string.Format("<p class=\"{0}\">", cssClass);

            string result = start + text
                 .Replace("\n\n", "</p>" + start)
                 .Replace("\n", "<br />")
                 .Replace("</p>" + start, "</p>" + Environment.NewLine + start) + "</p>";

            return new MvcHtmlString(result);
        }

        /// <summary>
        /// Returnerer "contentDate" (nyhed) som datetime. Hvis dato ikke er sat eller egenskaben "contentDate" ikke findes, returneres dokumentets oprettelsesdato.
        /// </summary>
        /// <param name="content">IPublishedContent</param>
        /// <returns></returns>
        public static DateTime Date(this IPublishedContent content)
        {
            if (content.HasProperty(Constants.SkyConstants.Properties.ContentDate))
            {
                var date = content.GetPropertyValue<DateTime>(Constants.SkyConstants.Properties.ContentDate);

                if (date != null && date != DateTime.MinValue)
                    return date;

                return content.CreateDate;
            }

            return content.CreateDate;
        }

        /// <summary>
        /// Returnerer dokumentets sidedato i formateret tilstand
        /// </summary>
        /// <param name="content">IPublishedContent</param>
        /// <returns></returns>
        public static string SiteDate(this IPublishedContent content)
        {
            return content.Date().SiteDate();
        }

        /// <summary>
        /// Returnerer en cropped url. Hvis egenskaben indeholder flere billeder, returneres det første billede
        /// </summary>
        /// <param name="content">IPublishedContent</param>
        /// <param name="property">Egenskabens alias</param>
        /// <param name="alias">CropUp alias</param>
        /// <returns></returns>
        public static string Image(this IPublishedContent content, string property, string alias)
        {
            var image = content.TypedCsvMedia(property).FirstOrDefault();

            if (image == null)
                return "";

            return image.GetCropUrl(propertyAlias: alias, preferFocalPoint: true).SupportWebP();
        }

        /// <summary>
        /// Returnerer en cropped url. Hvis egenskaben indeholder flere billeder, returneres det første billede
        /// </summary>
        /// <param name="content">IPublishedContent</param>
        /// <param name="property">Egenskabens alias</param>
        /// <param name="width">Bredde på CropUp</param>
        /// <param name="height">Højde på CropUp</param>
        /// <returns></returns>
        public static string Image(this IPublishedContent content, string property, int width, int height)
        {
            var image = content.TypedCsvMedia(property).FirstOrDefault();

            if (image == null)
                return "";

            return image.GetCropUrl(width, height, preferFocalPoint: true).SupportWebP();
        }

        /// <summary>
        /// Returnerer et IPublishedContent objekt af indstillingssiden
        /// </summary>
        /// <param name="helper">UmbracoHelper</param>
        /// <returns></returns>
        public static IPublishedContent SettingsPage(this UmbracoHelper helper)
        {
            return helper.ContentSingleAtXPath("//SOSU-SiteKontainer");
        }

        /// <summary>
        /// Returnerer li'er til at bygge simpel navigation (main- / second navigation). Bruges til at rendere topmenuer ud fra en MNTP egenskab.
        /// </summary>
        /// <param name="content">IPublishedContent</param>
        /// <param name="model">RenderModel</param>
        /// <param name="property">Egenskabens navn</param>
        /// <param name="activeClassName">Klassenavn på aktivt menupunkt</param>
        /// <returns></returns>
        public static string MakeNavItems(this IPublishedContent content, RenderModel model, string property, string activeClassName)
        {
            var r = new StringBuilder();

            var data = content.TypedCsvContent(property);

            foreach (var item in data)
            {
                var act = model.Content.IsDescendantOrSelf(item);

                r.AppendLine(string.Format("<li><a href=\"{0}\" class=\"{1}\">{2}</a></li>", item.Url, "test", item.Name));
            }

            return r.ToString();
        }

        /// <summary> 
        /// Returnere streng hvor ** er erstattet med <br /> og * erstattes af &shy; 
        /// </summary> 
        /// <param name="text"></param> 
        /// <returns></returns> 
        public static string Shyify(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            return text.Replace("**", "<br />").Replace("*", "&shy;");
        }

        /// <summary>
        /// Returnerer en liste af IPublishedContent objekter ud fra strukturen. Forsiden tilføjes ud fra indstillingssidens umbracoInternalRedirectId egenskab.
        /// </summary>
        /// <param name="content">IPublishedContent</param>
        /// <returns></returns>
        public static IEnumerable<IPublishedContent> BreadCrumb(this IPublishedContent content)
        {
            var helper = new UmbracoHelper(UmbracoContext.Current);

            var breadcrumb = new List<IPublishedContent>();

            var frontpage = helper.TypedContent(helper.SettingsPage().GetPropertyValue<int>(Constants.SkyConstants.Properties.UmbracoInternalRedirect));

            breadcrumb.Add(frontpage);

            breadcrumb.AddRange(from pathId in content.Path.Split(',') where pathId != "-1" select helper.TypedContent(pathId) into item where item.Level > 1 select item);

            return breadcrumb;
        }


        #region DateTime extensions

        public static string SiteDate(this DateTime date)
        {
            return date.ToString("d.MM.yyyy");
        }

        #endregion

        /// <summary> 
        /// Returnerer HtmlString hvor ** er erstattet med <br /> og * erstattes af &shy; 
        /// </summary> 
        /// <param name="content">IPublishedContent</param> 
        /// <param name="alias"></param> 
        /// <returns></returns> 
        public static HtmlString GetShyTitle(this IPublishedContent content, string alias = "shyTitle")
        {
            string alternate = content.GetPropertyValue<string>(alias);
            return new HtmlString(String.IsNullOrWhiteSpace(alternate) ? "" : HttpUtility.HtmlEncode(alternate).Replace("**", "<br />").Replace("*", "&shy;"));
        }

		/// <summary>
		/// If client supports webP add format to urlstring
		/// </summary>
		/// <param name="cropUrl"></param>
		/// <returns></returns>
		public static string SupportWebP(this string url)
		{
			if (string.IsNullOrWhiteSpace(url)) return "";

			bool supportsWebP = HttpContext.Current.Request.Headers["Accept"].ToLower().Contains("image/web");

			return string.Format("{0}{1}", url, supportsWebP ? "&format=webp" : "");
		}
	}
}
