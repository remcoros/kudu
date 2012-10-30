using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Security.Principal;

namespace DropBoxOAuth.Utility
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString DropBoxOptions(this HtmlHelper htmlHelper)
        {
            
            return htmlHelper.JsonIsland(new
            {
                baseUrl = "http://slumley-cloud:81/sean/defaultcollection",
                token = TokenManager.GetAccessToken("AUserTokenName")
            }, new { @class = "tfs-context" });
        }

        public static MvcHtmlString JsonIsland(this HtmlHelper htmlHelper, object data, object htmlAttributes)
        {
            return JsonIsland(htmlHelper, data, new RouteValueDictionary(htmlAttributes), 0);
        }

        public static MvcHtmlString JsonIsland(this HtmlHelper htmlHelper, object data, IDictionary<string, object> htmlAttributes, int maxJsonLength)
        {
            TagBuilder scriptTag = new TagBuilder("script");

            if (htmlAttributes != null)
            {
                scriptTag.MergeAttributes(htmlAttributes);
            }

            JavaScriptSerializer seralizer = new JavaScriptSerializer();

            // If a maxJsonLength was provided, then use it.
            if (maxJsonLength > 0)
            {
                seralizer.MaxJsonLength = maxJsonLength;
            }

            scriptTag.MergeAttribute("type", "application/json");
            scriptTag.MergeAttribute("defer", "defer");
            scriptTag.InnerHtml = seralizer.Serialize(data);

            return MvcHtmlString.Create(scriptTag.ToString());
        }
    }
}