using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DropBoxOAuth
{
    public static class TokenManager
    {
        static Dictionary<string, OauthTokenData> s_accessTokens = new Dictionary<string, OauthTokenData>();

        public static string GetAccessToken(string name)
        {
            OauthTokenData token;
            return s_accessTokens.TryGetValue(name, out token) ? token.access_token : null;
        }

        public static bool TryGetToken(string name, out OauthTokenData token)
        {
            return s_accessTokens.TryGetValue(name, out token);
        }

        public static void AddToken(string name, OauthTokenData token)
        {
            s_accessTokens[name] = token;
        }

        public static void RemoveToken(string name)
        {
            s_accessTokens.Remove(name);
        }
    }
}