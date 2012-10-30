using System;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.IO;

namespace DropBoxOAuth.Controllers
{
    public class WitController : Controller
    {
        //
        // GET: /Wit/

        public ActionResult Index()
        {
            if (TokenManager.GetAccessToken("AUserTokenName") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("index", "home");
            }
        }

        public ActionResult WorkItems(int id)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(CreateWitUrl("workitems") +"?ids="+id.ToString());
            webrequest.Method = "GET";
            webrequest.Headers.Add("Authorization: Bearer " + TokenManager.GetAccessToken("AUserTokenName"));
            JsObject result = new JsObject();
            try
            {
                HttpWebResponse hwrWebResponse = (HttpWebResponse)webrequest.GetResponse();
                if (hwrWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader srResponseReader = new StreamReader(hwrWebResponse.GetResponseStream());
                    string strResponseData = srResponseReader.ReadToEnd();
                    srResponseReader.Close();

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    return this.Json(serializer.DeserializeObject(strResponseData), JsonRequestBehavior.AllowGet);
                }
            }
            catch (WebException e)
            {
                if (e.Response.Headers.AllKeys.Contains("X-TFS-ServiceError"))
                {
                    result["success"] = false;
                    result["message"] = HttpUtility.UrlDecode(e.Response.Headers["X-TFS-ServiceError"]);
                    return this.Json(result, JsonRequestBehavior.AllowGet);
                }
                throw e;
            }

            result["success"] = false;
            result["message"] = "Unable to find work item";
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        //private String CreateWitUrl(string action)
        //{
        //    return String.Format("{0}/{1}/defaultcollection/{2}/_api/_wit/{3}", WebConfigurationManager.AppSettings["TfsUrl"],WebConfigurationManager.AppSettings["TfsAccount"], WebConfigurationManager.AppSettings["TeamProject"], action);
        //}

        private String CreateWitUrl(string action)
        {
            return String.Format("{0}/defaultcollection/{1}/_api/_wit/{2}", WebConfigurationManager.AppSettings["TfsUrl"], WebConfigurationManager.AppSettings["TeamProject"], action);
        }

        public ActionResult SetWebHookUrl(string url)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(CreateWitUrl("workitems") + "?ids=" + url.ToString());
            //webrequest.Method = "GET";
            //webrequest.Headers.Add("Authorization: Bearer " + TokenManager.GetAccessToken("AUserTokenName"));
            JsObject result = new JsObject();
            //try
            //{
            //    HttpWebResponse hwrWebResponse = (HttpWebResponse)webrequest.GetResponse();
            //    if (hwrWebResponse.StatusCode == HttpStatusCode.OK)
            //    {
            //        StreamReader srResponseReader = new StreamReader(hwrWebResponse.GetResponseStream());
            //        string strResponseData = srResponseReader.ReadToEnd();
            //        srResponseReader.Close();

            //        JavaScriptSerializer serializer = new JavaScriptSerializer();
            //        return this.Json(serializer.DeserializeObject(strResponseData), JsonRequestBehavior.AllowGet);
            //    }
            //}
            //catch (WebException e)
            //{
            //    if (e.Response.Headers.AllKeys.Contains("X-TFS-ServiceError"))
            //    {
            //        result["success"] = false;
            //        result["message"] = HttpUtility.UrlDecode(e.Response.Headers["X-TFS-ServiceError"]);
            //        return this.Json(result, JsonRequestBehavior.AllowGet);
            //    }
            //    throw e;
            //}

            result["success"] = false;
            result["message"] = "Unable to find work item";
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
