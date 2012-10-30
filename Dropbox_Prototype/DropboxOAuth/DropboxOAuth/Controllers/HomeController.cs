using System;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DropBoxOAuth.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            ViewBag.Message = "Welcome to ASP.NET MVC!";

            OauthTokenData token;
            if (TokenManager.TryGetToken("AUserTokenName", out token))
            {
                return RedirectToAction("Index", "wit");
            }
            else
            {

                return View();
            }
        }

        public ActionResult Reset()
        {
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            TokenManager.RemoveToken("AUserTokenName");

            return RedirectToAction("index");
        }

    }
}
