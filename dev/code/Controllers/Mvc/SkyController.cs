using System.Web.Mvc;
using code.Models.Website;
using Umbraco.Web.Mvc;

namespace code.Controllers.Mvc{
    public class SkyController : RenderMvcController{
        protected ViewResult View(Master model){
            return View(null, model);
        }

        protected ViewResult View(string view, Master model){
            return base.View(view, model);
        }
    }
}