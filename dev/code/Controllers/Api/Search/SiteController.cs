using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using code.Extensions;
using code.Models.Website.Search;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.Module.ErrorHandling;
using Skybrud.Umbraco.Search;
using Skybrud.Umbraco.Search.Models.Options;
using Skybrud.WebApi.Json;
using Skybrud.WebApi.Json.Meta;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;

namespace code.Controllers.Api.Search
{
	[JsonOnlyConfiguration]
	public class SiteController : UmbracoApiController
	{
		[System.Web.Http.HttpGet]
		public object Search(int siteid, string keywords = "", int offset = 0, int limit = 10)
		{
			HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

			if (!siteid.IsExistingNode())
			{
				return Request.CreateResponse(JsonMetaResponse.GetError("siteid is not valid"));
			}

			int total = 0;
			int pageTotal = 0;
			int jobTotal = 0;
			int employeeTotal = 0;
			try
			{
				#region Normal Search

				SearchOptions initialSo = new SearchOptions
				{
					Text = keywords,
					RootIds = new int[] { siteid },
					DocumentTypes = new string[] { Constants.SkyConstants.DocumentTypes.SubPage, Constants.SkyConstants.DocumentTypes.FrontPage },	// add your own doctypes
					Fields =
					{
						FieldList = new List<Field>
						{
							Field.GetFieldOption("nodeName_lci", 15, null),
							Field.GetFieldOption("title_lci", 15, null),
							Field.GetFieldOption("teaser_lci", 15, null),
							Field.GetFieldOption(Constants.SkyConstants.Properties.ContentGrid, null, null),
							// add more fields to search here
						 }
					},
					Order = new Order { OrderType = OrderType.Score },
					Limit = limit,
					Offset = offset,
					Debug = HttpContext.Current.IsDebuggingEnabled
				};

				List<IPublishedContent> results = SkybrudSearch.SearchDocuments(
					out pageTotal,
					initialSo
				).ToList();
				#endregion


				// create response body
				List<SiteSearchResult> returnResults = results.Select(x => SiteSearchResult.GetFromContent(x)).ToList();
				JsonMetaResponse response = JsonMetaResponse.GetSuccess(returnResults.Take(limit));

				total = pageTotal + jobTotal + employeeTotal;

				response.SetPagination(new JsonPagination
				{
					Total = total,
					Limit = limit,
					Offset = offset
				});

				//Set groups
				JObject obj = JObject.FromObject(response);


				return obj;

			}
			catch (Exception ex)
			{

				Error error = new Error("Der skete en fejl på serveren");
				LogHelper.Error<SiteController>(error.ToString(), ex);
				return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, error.Message, error));
			}
		}
	}
}
