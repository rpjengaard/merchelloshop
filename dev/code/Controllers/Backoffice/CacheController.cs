using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using code.Constants;
using Skybrud.WebApi.Json;
using Umbraco.Web.WebApi;

namespace code.Controllers.Backoffice
{
    [JsonOnlyConfiguration]
    public class CacheController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public object ClearAllCache()
        {
            ApplicationContext.ApplicationCache.RuntimeCache.ClearAllCache();

            return new
            {
                meta = new
                {
                    code = 200
                },
                data = true
            };
        }

        [HttpGet]
        public object ListAllCachedItems()
        {

            return new
            {
                meta = new
                {
                    code = 200
                },
                data = ApplicationContext.ApplicationCache.RuntimeCache.GetCacheItemsByKeySearch(SkyConstants.Prefix.CachePrefix).Select(x => x.ToString())
            };
        }

        [HttpGet]
        public object GetCacheTimeout()
        {

            return new
            {
                meta = new
                {
                    code = 200
                },
                data = GetCacheTimeoutInSecs()
            };
        }


        [HttpGet]
        public object SetCacheTimeout(int seconds)
        {

            // Map the path to the txt file
            string path = HostingEnvironment.MapPath("~/App_Data/SkyCacheTimeout.txt");

            File.WriteAllText(path, seconds + "");

            return new
            {
                meta = new
                {
                    code = 200
                },
                data = seconds
            };

        }

        public int GetCacheTimeoutInSecs()
        {
            // Map the path to the txt file
            string path = HostingEnvironment.MapPath("~/App_Data/SkyCacheTimeout.txt");

            int timeoutInSec = 0;

            if (File.Exists(path))
            {
                //read content
                int.TryParse(File.ReadAllText(path), out timeoutInSec);
            }
            else
            {
                File.WriteAllText(path, "3600");
                timeoutInSec = 3600;
            }

            return timeoutInSec;
        }
    }
}