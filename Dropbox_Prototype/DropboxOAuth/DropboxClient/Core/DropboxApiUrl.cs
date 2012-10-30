using System;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class DropboxApiUrl
    {
        /// <summary>
        /// 
        /// </summary>
        public class Version0
        {
            /// <summary>
            /// 
            /// </summary>
            public class OAuth
            {
                /// <summary>
                /// 
                /// </summary>
                public static readonly String RequestToken = "api.dropbox.com/0/oauth/request_token";
                /// <summary>
                /// 
                /// </summary>
                public static readonly String AuthorizeToken = "www.dropbox.com/0/oauth/authorize";
                /// <summary>
                /// 
                /// </summary>
                public static readonly String AccessToken = "api.dropbox.com/0/oauth/access_token";
            }
            /// <summary>
            /// 
            /// </summary>
            public static readonly String Token = "api.dropbox.com/0/token";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String GetAccount = "api.dropbox.com/0/account/info";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String CreateAccount = "api.dropbox.com/0/account";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String Files = "api-content.dropbox.com/0/files/dropbox";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String Metadata = "api.dropbox.com/0/metadata/dropbox";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String Thumbnail = "api-content.dropbox.com/0/thumbnails/dropbox";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String CopyFile = "api.dropbox.com/0/fileops/copy";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String MoveFile = "api.dropbox.com/0/fileops/move";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String CreateFolder = "api.dropbox.com/0/fileops/create_folder";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String Delete = "api.dropbox.com/0/fileops/delete";
        }
        /// <summary>
        /// 
        /// </summary>
        public class Version1
        {
            /// <summary>
            /// 
            /// </summary>
            public class OAuth
            {
                /// <summary>
                /// 
                /// </summary>
                public static readonly String RequestToken = "api.dropbox.com/1/oauth/request_token";
                /// <summary>
                /// 
                /// </summary>
                public static readonly String AuthorizeToken = "www.dropbox.com/1/oauth/authorize";
                /// <summary>
                /// 
                /// </summary>
                public static readonly String AccessToken = "api.dropbox.com/1/oauth/access_token";
            }
            /// <summary>
            /// 
            /// </summary>
            public static readonly String Token = "api.dropbox.com/1/token";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String GetAccount = "api.dropbox.com/1/account/info";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String CreateAccount = "api.dropbox.com/1/account";
            /// <summary>
            /// 
            /// </summary>
            // 2012/03/31 SH.M  Edit Start
            //public static readonly String Files = "api-content.dropbox.com/1/files/dropbox";
            public static readonly String Files = "api-content.dropbox.com/1/files/{0}";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String FilesPut = "api-content.dropbox.com/1/files_put/{0}";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String Delta = "api.dropbox.com/1/delta";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String Revision = "api.dropbox.com/1/revisions/{0}";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String Restore = "api.dropbox.com/1/restore/{0}";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String Search = "api.dropbox.com/1/search/{0}";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String Share = "api.dropbox.com/1/shares/{0}";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String Media = "api.dropbox.com/1/media/{0}";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String CopyRef = "api.dropbox.com/1/copy_ref/{0}";
            /// <summary>
            /// 
            /// </summary>
            //public static readonly String Metadata = "api.dropbox.com/1/metadata/dropbox";
            public static readonly String Metadata = "api.dropbox.com/1/metadata/{0}";
            /// <summary>
            /// 
            /// </summary>
            //public static readonly String Thumbnail = "api-content.dropbox.com/1/thumbnails/dropbox";
            public static readonly String Thumbnail = "api-content.dropbox.com/1/thumbnails/{0}";
            // Edit End
            /// <summary>
            /// 
            /// </summary>
            public static readonly String CopyFile = "api.dropbox.com/1/fileops/copy";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String MoveFile = "api.dropbox.com/1/fileops/move";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String CreateFolder = "api.dropbox.com/1/fileops/create_folder";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String Delete = "api.dropbox.com/1/fileops/delete";
        }
        //edit start 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <param name="addPath"></param>
        /// <returns></returns>
        public static String GetApiUrl(String apiUrl, String addPath)
        {
            return GetApiUrl(apiUrl, "", addPath);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <param name="param"></param>
        /// <param name="addPath"></param>
        /// <returns></returns>
        public static String GetApiUrl(String apiUrl, String param, String addPath)
        {
            return GetApiUrl(apiUrl, new[] { param }, addPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <param name="params"> </param>
        /// <param name="addPath"></param>
        /// <returns></returns>
        public static String GetApiUrl(String apiUrl, String[] @params, String addPath)
        {
            if (!addPath.StartsWith("/"))
            {
                addPath = "/" + addPath;
            }
            if (@params != null)
            {
                apiUrl = String.Format(apiUrl, @params);
            }
            var result = apiUrl + addPath;
            return result.ToLower();
        }
    }
}
