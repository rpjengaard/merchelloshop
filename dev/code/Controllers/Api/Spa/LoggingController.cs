using System.Configuration;
using System.Web;
using System.Web.Http;
using Skybrud.Umbraco.Module.Slack;
using Skybrud.WebApi.Json;
using Umbraco.Web.WebApi;

namespace code.Controllers.Api.Spa
{
	[JsonOnlyConfiguration]
	public class LoggingController : UmbracoApiController
	{
		[HttpGet]
		public object SkyLogError(string errorMsg)
		{
			HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

			//string referrer = Request.UrlReferrer();
			string fErrorMsg = HttpContext.Current.Server.UrlDecode(errorMsg);

			if (bool.Parse(ConfigurationManager.AppSettings.Get("skyViewLoggingActivateSlack")))
			{
				if (!string.IsNullOrWhiteSpace(fErrorMsg))
				{
					SlackUtils.SendMsgToChannel(fErrorMsg, true);
				}
			}

			return true;
		}
	}
}
