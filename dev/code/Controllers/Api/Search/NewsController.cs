using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using code.Extensions;
using code.Models.Website.Search;
using Skybrud.Umbraco.Module.ErrorHandling;
using Skybrud.Umbraco.Search.Models.Interfaces;
using Skybrud.Umbraco.Search.Models.Options;
using Skybrud.WebApi.Json;
using Skybrud.WebApi.Json.Meta;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;

namespace code.Controllers.Api.Search
{
	[JsonOnlyConfiguration]
	public class NewsController : UmbracoApiController
	{
		[System.Web.Http.HttpGet]
		public object Search(int contextId, string keywords = "", int offset = 0, int limit = 10, string year = "")
		{
			HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

			if (!contextId.IsExistingNode())
			{
				return Request.CreateResponse(JsonMetaResponse.GetError("contextId is not a valid Umbraco-node"));
			}

			List<IPublishedContent> results;
			int total;
			try
			{

				SpecificFields fields = new SpecificFields
				{
					Fields = new List<ISpecificField>
					{
						new SpecificField
						{
							FieldName = "year",
							SearchTerms = new []{year}
						}
					}
				};
				SearchOptions initialSo = new SearchOptions
				{
					Text = keywords,
					DocumentTypes = new string[] { Constants.SkyConstants.DocumentTypes.NewsPage },
					RootIds = new int[] { contextId },
					Fields =
				{
					FieldList = new List<Field>
					{
						Field.GetFieldOption("nodeName_lci", 15, null),
						Field.GetFieldOption("titel_lci", 15, null),
						Field.GetFieldOption("teaser_lci", 15, null),
						Field.GetFieldOption(Constants.SkyConstants.Properties.ContentGrid, null, null)
					}
				},
					Order = new Order()
					{
						FieldName = string.Format("{0}_range", Constants.SkyConstants.Properties.ContentDate),
						OrderDirection = OrderDirection.Descending,
						OrderType = OrderType.String
					},
					DateRange = new DateRange()
					{
						FieldName = Constants.SkyConstants.Properties.ContentDate,
						End = DateTime.MaxValue,
						Start = DateTime.MinValue.AddDays(1)
					},
					Limit = limit,
					Offset = offset,
					Debug = HttpContext.Current.IsDebuggingEnabled
				};
				if (!string.IsNullOrEmpty(year))
				{
					initialSo.SpecificFields = fields;
				}
				results = Skybrud.Umbraco.Search.SkybrudSearch.SearchDocuments(
				   out total,
				   initialSo
				).ToList();


			}
			catch (Exception ex)
			{
				Error error = new Error("Der skete en fejl på serveren");
				LogHelper.Error<NewsController>(error.ToString(), ex);
				return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, error.Message, error));
			}
			var returnResults = results.Select(x => NewsSearchResult.GetFromContent(x));
			JsonMetaResponse response = JsonMetaResponse.GetSuccess(returnResults);

			response.SetPagination(new JsonPagination
			{
				Total = total,
				Limit = limit,
				Offset = offset
			});
			return Request.CreateResponse(response);
		}


	}
}
