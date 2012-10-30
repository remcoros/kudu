using System;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Configuration;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.IO;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.ApplicationBlock;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using System.Net.Http;

namespace DropBoxOAuth.Controllers
{
    public class WebSitesController : Controller
    {
        // http://localhost
        //const string ConsumerKey = "cb77ca93901d4e30a162d85404637528";
        //const string ConsumerSecret = "5daac2424e4242bb8fdebc8a3bc29fea";

        const string ConsumerKey = "hbbdkdh8y0qigrn";
        const string ConsumerSecret = "to6vjhxko848lgx";

        //const string Protocol = "https";
        //const string Project = "TestOAuth";
        //const string HostName = "www.DropBox.com";

        static InMemoryTokenManager inMemoryTokenManager = new InMemoryTokenManager(ConsumerKey, ConsumerSecret);

        public ActionResult RequestToken()
        {
            //GetAccountInfo("w7fv41cg30gg1jo", "ka9n4keicpztufu");

            //POST https://api.dropbox.com/1/oauth/request_token
            //Your HTTP request should have the following header:
            //Authorization: OAuth oauth_version="1.0", oauth_signature_method="PLAINTEXT", oauth_consumer_key="<app-key>", oauth_signature="<app-secret>&"
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.dropbox.com/");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", 
                string.Format("oauth_version=\"1.0\", oauth_signature_method=\"PLAINTEXT\", oauth_consumer_key=\"{0}\", oauth_signature=\"{1}&\"", ConsumerKey, ConsumerSecret));
            HttpResponseMessage response = client.PostAsync("/1/oauth/request_token", null).Result.EnsureSuccessStatusCode();
            string result = response.Content.ReadAsStringAsync().Result;

            //WebConsumer consumer = CreateConsumer();
            UriBuilder callback = new UriBuilder(new Uri(
                Request.Url.AbsoluteUri.Replace("RequestToken", "TokenAuthorize")));
            callback.Query = "";
            //var requestParams = new Dictionary<string, string>(); // { { "scope", "project_info,test_web_hook" } };
            //var response = consumer.PrepareRequestUserAuthorization(callback.Uri, requestParams, null);
            // TODO: Redirect happens auto-magically
            //consumer.Channel.Send(response);

            var pair = ParseResult(result);
            string token = pair["oauth_token"];
            string tokenSecret = inMemoryTokenManager.tokensAndSecrets[token] = pair["oauth_token_secret"];

            HttpCookie cookie = new HttpCookie("DropBox_oauth");
            //cookie.Secure = true;
            cookie.HttpOnly = true;
            cookie.Values[Url.Encode(token)] = tokenSecret;
            cookie.Expires = DateTime.UtcNow.AddSeconds(30);
            HttpContext.Response.Cookies.Set(cookie);

            // https://www.dropbox.com/1/oauth/authorize
            UriBuilder uriBuilder = new UriBuilder("https://www.dropbox.com/");
            uriBuilder.Path = "/1/oauth/authorize";
            uriBuilder.AppendQueryArgument("oauth_token", token);
            // This is out-of-spec of OAuth1.0
            uriBuilder.AppendQueryArgument("oauth_callback", callback.Uri.AbsoluteUri);
            return new RedirectResult(uriBuilder.Uri.AbsoluteUri);
        }

        static Dictionary<string, string> ParseResult(string result)
        {
            var pair = new Dictionary<string, string>();
            foreach (var setting in result.Split('&'))
            {
                string[] keyval = setting.Split('=');
                pair[keyval[0]] = keyval[1];
            }
            return pair;
        }

        public ActionResult TokenAuthorize(string uid, string oauth_token)
        {
            if (!String.IsNullOrEmpty(oauth_token))
            {
                HttpCookie cookie = HttpContext.Request.Cookies.Get("DropBox_oauth");
                Console.WriteLine(cookie.Expires);
                string tokenSecret = cookie.Values[Url.Encode(oauth_token)];


                //POST https://api.dropbox.com/1/oauth/access_token
                //This token is what lets you make calls to the Dropbox API. Your HTTP request should have the following header:
                //Authorization: OAuth oauth_version="1.0", oauth_signature_method="PLAINTEXT", oauth_consumer_key="<app-key>", oauth_token="<request-token>", oauth_signature="<app-secret>&<request-token-secret>"

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://api.dropbox.com/");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth",
                    string.Format("oauth_version=\"1.0\", oauth_signature_method=\"PLAINTEXT\", oauth_consumer_key=\"{0}\", oauth_token=\"{1}\", oauth_signature=\"{2}&{3}\"", ConsumerKey, oauth_token, ConsumerSecret, tokenSecret));
                HttpResponseMessage response = client.PostAsync("/1/oauth/access_token", null).Result.EnsureSuccessStatusCode();
                string result = response.Content.ReadAsStringAsync().Result;
                var pair = ParseResult(result);
                var token = pair["oauth_token"];
                tokenSecret = inMemoryTokenManager.tokensAndSecrets[token] = pair["oauth_token_secret"];

                GetAccountInfo(token, tokenSecret);

                //WebConsumer consumer = CreateConsumer();
                //var accessTokenMessage = consumer.ProcessUserAuthorization();
                //if (accessTokenMessage != null)
                //{
                //    OauthTokenData token = new OauthTokenData
                //    {
                //        access_token = accessTokenMessage.AccessToken
                //    };
                //    TokenManager.AddToken("AUserTokenName", token);

                //    //ListRepos(token);
                //    ////    GetHookUrl(token);
                //    //SetupHookUrl(token);
                //    //TestHook(token);
                //}

                return RedirectToAction("Index", "home");
            }
            else
            {
                // Decline
                return RedirectToAction("Index", "home");
            }
            
        }

        private void GetAccountInfo(string token, string tokenSecret)
        {
            //GET https://api.dropbox.com/1/account/info
            //Your HTTP request should have the following header:
            //Authorization: OAuth oauth_version="1.0", oauth_signature_method="PLAINTEXT", oauth_consumer_key="<app-key>", oauth_token="<access-token>, oauth_signature="<app-secret>&<access-token-secret>"

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.dropbox.com/");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth",
                string.Format("oauth_version=\"1.0\", oauth_signature_method=\"PLAINTEXT\", oauth_consumer_key=\"{0}\", oauth_token=\"{1}\", oauth_signature=\"{2}&{3}\"", ConsumerKey, token, ConsumerSecret, tokenSecret));
            HttpResponseMessage response = client.GetAsync("/1/account/info").Result.EnsureSuccessStatusCode();
            string result = response.Content.ReadAsStringAsync().Result;
        }

        private WebConsumer CreateConsumer()
        {
            var tokenManager = inMemoryTokenManager;
            var requestTokenEndpoint = new MessageReceivingEndpoint(
                new Uri("https://api.dropbox.com/1/oauth/request_token"),
                HttpDeliveryMethods.PostRequest);
            var userAuthorizationEndpoint = new MessageReceivingEndpoint(
                new Uri("https://www.dropbox.com/1/oauth/authorize"),
                HttpDeliveryMethods.PostRequest);
            var accessTokenEndpoint = new MessageReceivingEndpoint(
                new Uri("https://api.dropbox.com/1/oauth/access_token"),
                HttpDeliveryMethods.PostRequest);
            WebConsumer consumer = new WebConsumer(
                new ServiceProviderDescription
                {
                    RequestTokenEndpoint = requestTokenEndpoint,
                    UserAuthorizationEndpoint = userAuthorizationEndpoint,
                    AccessTokenEndpoint = accessTokenEndpoint,
                    ProtocolVersion = ProtocolVersion.V10a,
                    TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() }
                },
                tokenManager);

            return consumer;
        }

    }
}
