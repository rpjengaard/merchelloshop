using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using code.Constants;
using code.Extensions;
using code.Helpers;
using code.Models.Spa;
using code.Models.Spa.Content;
using code.Models.Spa.Navigation;
using code.Models.Spa.Site;
using code.SkybrudSpaPackage.Api;
using code.SkybrudSpaPackage.Models;
using Skybrud.UmbracoEssentials.Extensions.PublishedContent;
using Skybrud.WebApi.Json;
using Skybrud.WebApi.Json.Meta;
using umbraco.cms.businesslogic.web;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;
using SpaApiPart = code.SkybrudSpaPackage.Models.SpaApiPart;

namespace code.Controllers.Api.Spa
{
    [JsonOnlyConfiguration]
    public class SpaController : SpaApiControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">url you try to get content from</param>
        /// <param name="parts">what parts in the api you want to return (content, navigation, site)</param>
        /// <param name="navLevels">1/2 (1 = default)</param>
        /// <param name="navContext">set to true to get navigation context on initial request</param>
        /// <param name="nodeId">overwrites url-property if nodeId is indicated</param>
        /// <param name="appSiteId">Id of the site in Umbraco (old property-name: umbracoSiteId)</param>
        /// <param name="appHost">Domain of the SPA Client (old property-name: spaClientDomain)</param>
        /// <param name="appProtocol">Scheme of the SPA Client (http / https) (old property-name: spaClientScheme)</param>
        /// <returns></returns>
        [HttpGet]
		[HttpOptions]
		public object GetData(string url = "", [FromUri] string parts = "", int navLevels = 1, bool navContext = false, int nodeId = -1, int appSiteId = -1, string appHost = "", string appProtocol = "") {

            // Use the current URL as fallback for "appHost" and "appProtocol"
            appHost = String.IsNullOrWhiteSpace(appHost) ? Request.RequestUri.Host : appHost;
            appProtocol = String.IsNullOrWhiteSpace(appProtocol) ? Request.RequestUri.Scheme : appProtocol;

            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

            Stopwatch watch = Stopwatch.StartNew();

            HttpStatusCode statusCode = HttpStatusCode.OK;

            try {

                HttpResponseMessage response;
                
                // if nodeId exists, prefer content from that node
				if (nodeId > 0) {
					IPublishedContent c = UmbracoContext.Current.ContentCache.GetById(nodeId);

					if (c != null) {
						url = c.Url;
					}
				}

                // Try get siteId from domain
                if (appSiteId == -1 && !string.IsNullOrWhiteSpace(appHost)) appSiteId = Domain.GetRootFromDomain(appHost);
            
                
                
                #region Setup
                

                // Get a reference to the site node
                IPublishedContent site = UmbracoContext.ContentCache.GetById(appSiteId);
                if (site == null) return ReturnServerError();

	            // Parse the options from the query string
	            SpaApiRequest options = new SpaApiRequest(appSiteId, url, parts) { Protocol = appProtocol, HostName = appHost };

	            if (BeforeSetup(options, site, out response)) return response;


                #region Content lookup
                
                IPublishedContent content = null;

                if (options.IsPreview) {

                    // Get the ID from the request
                    int id = url.GetPreviewId();

                    // Attempt to set the preview context
                    if (SpaHelpers.SetPreviewContext(id)) {

                        // Look up the page in the content cache (with preview enabled)
                        content = UmbracoContext.ContentCache.GetById(true, id);

                    }

                    // Show the 404 page if we don't have a content page at this point
                    if (content == null) {

                        // Get the culture ID
                        int cultureNodeId = GetCultureIdFromUrl(options);

                        // Lookup the culture node in the content cache
                        IPublishedContent cultureNode = UmbracoContext.ContentCache.GetById(cultureNodeId);

						

                        // Look for a 404 page configuration on the culture node
                        if (cultureNode != null)
                            content = cultureNode.TypedContent(SkyConstants.Properties.NotFoundPage);

                        // If we have no content at this point, we just return a simple 
                        if (content == null) return CreateSpaResponse(JsonMetaResponse.GetError(statusCode, "Page not found"));

                    }

                } else {

                    // Get a reference to the current page (fetched regardless of "parts" as the URL determines the culture)
                    content = GetContentFromInput(site, nodeId, url);

                    // Handle "umbracoInternalRedirectId" when present
                    if (content != null && content.HasValue(global::Umbraco.Core.Constants.Conventions.Content.InternalRedirectId)) {
                        content = Umbraco.TypedContent(global::Umbraco.Core.Constants.Conventions.Content.InternalRedirectId);
                    }

                }

                #endregion

                #region Handle any 404/redirects URLs

                if (content == null) {

                    // Got any redirects?
                    if (HandleRedirects(options, out response)) return response;

                    // Set the status code (404)
                    statusCode = HttpStatusCode.NotFound;

                    // Get the culture ID
                    int cultureNodeId = GetCultureIdFromUrl(options);

                    // Lookup the culture node in the content cache
                    IPublishedContent cultureNode = UmbracoContext.ContentCache.GetById(cultureNodeId);

                    // Look for a 404 page configuration on the culture node
                    if (cultureNode != null) content = cultureNode.GetPropertyValue<IPublishedContent>(SkyConstants.Properties.NotFoundPage);
                        
                    // If we have no content at this point, we just return a simple 
                    if (content == null) return CreateSpaResponse(JsonMetaResponse.GetError(statusCode, "Page not found"));

                }

                #endregion
                
                // If "content" matches the locale node, we get the content of the front page instead
                if (content.DocumentTypeAlias == SkyConstants.DocumentTypes.Culture) {
                    content = content.FirstChild() ?? content;
                }

                // Set the current culture
                Thread.CurrentThread.CurrentCulture = content.GetCulture();
	            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

                // Initialize the main model
                SpaPartDataModel model = new SpaPartDataModel(appSiteId, content.Id);

                // Get a reference to the culture node (eg. "Dansk" or "English")
                IPublishedContent culture = content.AncestorOrSelf(2);

                // Initialize the site/culture settings
                SpaSiteModel siteModel = new SpaSiteModel(site, culture);
                
                #endregion

                #region Content
                
                if (options.Parts.Contains(SpaApiPart.Content)) {
                    SpaContentModel spaContent = new SpaContentModel(siteModel, appHost, appProtocol);
                    model.Content = spaContent.GetContent(siteModel, content);
                }

                #endregion
                
                #region Navigation

                if (options.Parts.Contains(SpaApiPart.Navigation)) {
	                // to handle navigation right, we try to load the initial nodeId, so we dont get FirstChild on cultureNode
	                //IPublishedContent navContent = UmbracoContext.Current.ContentCache.GetById(nodeId) ?? content;
                    model.Navigation = new SpaNavigationModel(content, navLevels, navContext);
                }

                #endregion

                #region Site

                if (options.Parts.Contains(SpaApiPart.Site) && appSiteId > 0) {
                    model.Site = siteModel;
                }

                #endregion
                
                watch.Stop();
                model.ExecuteTimeMs = watch.ElapsedMilliseconds;

                return CreateSpaResponse(statusCode, model);


            } catch (Exception ex) {
				LogHelper.Error<SpaController>("SpaController Exception: ", ex);
                return ReturnServerError();
            }

		}


		[System.Web.Http.HttpGet]
		public object GetPreviewStatus() {
			return CreateSpaResponse(UmbracoContext.Current.InPreviewMode);
		}

		[System.Web.Http.HttpGet]
		public object GetCurrentUser() {
			return CreateSpaResponse(UmbracoContext.Current.Security.CurrentUser);
		}

		[System.Web.Http.HttpGet]
		public object GetUmbracoUser() {
			return CreateSpaResponse(Security.CurrentUser.Username);
		}

        #region Methods from SpaApiControllerBase

        protected override int GetCultureIdFromUrl(SpaApiRequest request)
        {

	        string culture = request.Url.Split('/')[1];

	        if (culture == "da")
	        {
				return SkyConstants.Pages.Danish.Culture;
	        }
	        else
	        {
		        return SkyConstants.Pages.English.Culture;
	        }

        }

        protected bool BeforeSetup(SpaApiRequest request, IPublishedContent site, out HttpResponseMessage response) {

            // Make sure "response" is initialized
            response = null;

		    // Redirect the the user to the Danish site if the root of the domain is requested
		    if (request.Url == "/")
		    {

			    List<IPublishedContent> startNode = site.HasValue(SkyConstants.Properties.StartNode)
				    ? site.GetPropertyValue<List<IPublishedContent>>(SkyConstants.Properties.StartNode)
				    : null;

			    string defaultUrl = startNode != null && startNode.Any() ? startNode.First().Url : "/en/";
			    response = ReturnRedirect(request, $"{request.Protocol}://{request.HostName}{defaultUrl}");
			    return true;
		    }

            return false;

        }

        #endregion

    }

}