using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DropboxClient : OAuthClient
    {
        private HttpProtocolType _protocol = HttpProtocolType.Https;
        private String _accessToken = "";
        private String _accessTokenSecret = "";

        /// <summary>
        /// 
        /// </summary>
        public HttpProtocolType Protocol
        {
            get { return _protocol; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String AccessToken
        {
            get { return _accessToken; }
            set { _accessToken = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String AccessTokenSecret
        {
            get { return _accessTokenSecret; }
            set { _accessTokenSecret = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumerKey"></param>
        /// <param name="consumerSecret"></param>
        /// <param name="accessToken"></param>
        /// <param name="accessTokenSecret"></param>
        public DropboxClient(String consumerKey, String consumerSecret, String accessToken, String accessTokenSecret)
            : base(consumerKey, consumerSecret
                , "https://" + DropboxApiUrl.Version1.OAuth.RequestToken
                , "https://" + DropboxApiUrl.Version1.OAuth.AuthorizeToken
                , "https://" + DropboxApiUrl.Version1.OAuth.AccessToken)
        {
            _accessToken = accessToken;
            _accessTokenSecret = accessTokenSecret;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumerKey"></param>
        /// <param name="consumerSecret"></param>
        /// <returns></returns>
        public static OAuthClient CreateOAuthClient(String consumerKey, String consumerSecret)
        {
            var cl = new OAuthClient(consumerKey, consumerSecret
                , "https://" + DropboxApiUrl.Version1.OAuth.RequestToken
                , "https://" + DropboxApiUrl.Version1.OAuth.AuthorizeToken
                , "https://" + DropboxApiUrl.Version1.OAuth.AccessToken);
            return cl;
        }
        private void GetResponseBodyText(String url, DropboxCommand command, Action<String> callback)
        {
            this.GetResponseBodyText(HttpMethodName.Get, url, command, callback);
        }
        private void GetResponseBodyText(HttpMethodName methodName, String url, DropboxCommand command, Action<String> callback)
        {
            this.GetHttpWebResponse(methodName, url, command, res => callback(res.BodyText));
        }
        private void GetHttpWebResponse(HttpMethodName methodName, String url, DropboxCommand command, Action<Byte[]> callback)
        {
            this.GetHttpWebResponse(methodName, url, command, res => callback(res.BodyData));
        }
        private void GetHttpWebResponse(HttpMethodName methodName, String url, DropboxCommand command, Action<HttpResponse> callback)
        {
            String u = _protocol.ToString().ToLower() + "://" + url;
            IDictionary<String, String> d = new Dictionary<String, String>();

            if (methodName == HttpMethodName.Put ||
                methodName == HttpMethodName.Delete)
            { throw new ArgumentException(); }

            if (command != null)
            {
                d = command.GetParameters();
            }
            this.GetHttpWebResponse(methodName, u, this._accessToken, this._accessTokenSecret, d
                , res => this.GetHttpWebResponseCallback(res, callback));
        }
        private void GetHttpWebResponseCallback(HttpWebResponse response, Action<HttpResponse> callback)
        {
            var res = new HttpResponse(response, this.ResponseEncoding);
            callback(res);
        }
        // edit start
        //private void Post(String url, UploadFileCommand command, Action<HttpResponse> callback)
        private void Post(String url, UploadFileCommand command, Action<String> callback)
        {
            var u = _protocol.ToString().ToLower() + "://" + url;
            var d = command.GetParameters();
            d["file"] = command.FileName;
            var cm = this.CreateHttpRequestCommand(HttpMethodName.Post, u, this.AccessToken, this.AccessTokenSecret, d);
            var boundary = UploadFileCommand.GetBoundaryString();
            cm.ContentType = "multipart/form-data; boundary=\"" + boundary + "\"";
            cm.SetBodyStream(command.CreatePostData(boundary));
            this.GetResponse(cm, res => callback(res.BodyText));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumerKey"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="callback"></param>
        public static void GetAccessTokenInfo(String consumerKey, String email, String password, Action<AccessTokenInfo> callback)
        {
            GetAccessTokenInfo(consumerKey, email, password, callback, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumerKey"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="callback"></param>
        /// <param name="errorCallback"></param>
        public static void GetAccessTokenInfo(String consumerKey, String email, String password
            , Action<AccessTokenInfo> callback, EventHandler<AsyncHttpCallErrorEventArgs> errorCallback)
        {
            var cl = new HttpClient();
            if (errorCallback != null)
            {
                cl.Error += errorCallback;
            }
            String url = String.Format("https://{0}?oauth_consumer_key={1}&email={2}&password={3}"
                , DropboxApiUrl.Version1.Token, consumerKey, email, password);
            cl.GetBodyText(url, res =>
            {
                var d = JsonConvert.DeserializeObject<Dictionary<String, Object>>(res);
                callback(new AccessTokenInfo(d["token"].ToString(), d["secret"].ToString()));
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="command"></param>
        public void GetAccountInfo(GetAccountCommand command, Action<AccountInfo> callback)
        {
            this.GetResponseBodyText(DropboxApiUrl.Version1.GetAccount, command, text => callback(new AccountInfo(text)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callback"></param>
        public void CreateAccount(CreateAccountCommand command, Action<String> callback)
        {
            this.GetResponseBodyText(DropboxApiUrl.Version1.CreateAccount, command, callback);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callback"></param>
        public void GetMetadata(GetMetadataCommand command, Action<Metadata> callback)
        {
            var url = DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Metadata, command.Root.ToString(), command.Path);
            this.GetResponseBodyText(url, command, text => callback(new Metadata(text)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// 
        public void GetFile(String path, Action<Byte[]> callback)
        {
            this.GetFile(path, RootFolder.Dropbox, callback);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="root"></param>
        /// <param name="callback"></param>
        public void GetFile(String path, RootFolder root, Action<Byte[]> callback)
        {
            this.GetFile(new GetFileCommand { Path = path, Root = root }
                , callback);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="callback"></param>
        public void GetFile(GetFileCommand cmd, Action<Byte[]> callback)
        {
            var url = DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Files, cmd.Root.ToString(), cmd.Path);
            this.GetHttpWebResponse(HttpMethodName.Get, url, cmd, callback);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callback"></param>
        public void UploadFile(UploadFileCommand command, Action<Metadata> callback)
        {
            if (String.IsNullOrEmpty(command.FolderPath)) { throw new ArgumentException(); }
            string url = DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Files, command.Root.ToString(), command.FolderPath);
            this.Post(url, command, text => callback(new Metadata(text)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callback"></param>
        public void GetThumbnails(GetThumbnailsCommand command, Action<Byte[]> callback)
        {
            this.GetHttpWebResponse(HttpMethodName.Get
                , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Thumbnail, command.Root.ToString(), command.Path)
                , command
                , callback);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callback"></param>
        public void CopyFile(MoveFileCommand command, Action<Metadata> callback)
        {
            this.GetResponseBodyText(HttpMethodName.Post, DropboxApiUrl.Version1.CopyFile, command, text => callback(new Metadata(text)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callback"></param>
        public void MoveFile(MoveFileCommand command, Action<Metadata> callback)
        {
            this.GetResponseBodyText(HttpMethodName.Post, DropboxApiUrl.Version1.MoveFile, command, text => callback(new Metadata(text)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callback"></param>
        public void CreateFolder(CreateFolderCommand command, Action<Metadata> callback)
        {
            this.GetResponseBodyText(HttpMethodName.Post, DropboxApiUrl.Version1.CreateFolder, command, text => callback(new Metadata(text)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callback"></param>
        public void Delete(DeleteCommand command, Action<Metadata> callback)
        {
            this.GetResponseBodyText(HttpMethodName.Post, DropboxApiUrl.Version1.Delete, command, text => callback(new Metadata(text)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callback"></param>
        public void PutFile(PutFileCommand command, Action<Metadata> callback)
        {
            if (String.IsNullOrEmpty(command.Path)) { throw new ArgumentException(); }
            var url = DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.FilesPut, command.Root.ToString(), command.Path);
            url = _protocol.ToString().ToLower() + "://" + url;
            var cm = this.CreateHttpRequestCommand(HttpMethodName.Put, url, this.AccessToken, this.AccessTokenSecret, command.GetParameters());
            cm.SetBodyStream(command.CreatePutData());
            this.GetResponse(cm, res => callback(new Metadata(res.BodyText)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="callback"></param>
        public void GetRevisions(GetRevisionsCommand cmd, Action<List<Metadata>> callback)
        {
            this.GetResponseBodyText(HttpMethodName.Get
                    , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Revision, cmd.Root.ToString(), cmd.Path)
                    , cmd
                    , text => callback(GetMetadataList(text)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="callback"></param>
        public void Search(SearchCommand cmd, Action<List<Metadata>> callback)
        {
            this.GetResponseBodyText(HttpMethodName.Post
                    , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Search, cmd.Root.ToString(), cmd.Path)
                    , cmd
                    , text => callback(GetMetadataList(text)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="callback"></param>
        public void Restore(RestoreCommand cmd, Action<Metadata> callback)
        {
            this.GetResponseBodyText(HttpMethodName.Post
                , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Restore, cmd.Root.ToString(), cmd.Path)
                , cmd
                , text => callback(new Metadata(text)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="callback"></param>
        public void GetShareableLink(SharesCommand cmd, Action<LinkInfo> callback)
        {
            this.GetResponseBodyText(HttpMethodName.Post
                , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Share, cmd.Root.ToString(), cmd.Path)
                , cmd
                , text => callback(new LinkInfo(text)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="callback"></param>
        public void GetMediaLink(MediaCommand cmd, Action<LinkInfo> callback)
        {
            this.GetResponseBodyText(HttpMethodName.Post
                , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Media, cmd.Root.ToString(), cmd.Path)
                , cmd
                , text => callback(new LinkInfo(text)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="callback"></param>
        public void GetCopyRef(GetCopyRefCommand cmd, Action<CopyRefInfo> callback)
        {
            this.GetResponseBodyText(HttpMethodName.Get
                , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.CopyRef, cmd.Root.ToString(), cmd.Path)
                , cmd
                , text => callback(new CopyRefInfo(text)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="callback"></param>
        public void GetDelta(GetDeltaCommand cmd, Action<DeltaInfo> callback)
        {
            this.GetResponseBodyText(HttpMethodName.Post
                , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Delta, "")
                , cmd
                , text => callback(new DeltaInfo(text)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsontext"></param>
        /// <returns></returns>
        private List<Metadata> GetMetadataList(String jsontext)
        {
            var result = new List<Metadata>();
            var ja = JsonConvert.DeserializeObject(jsontext) as JArray;

            if (ja != null)
            {
                result.AddRange(ja.Select(s => new Metadata(s.ToString())));
            }
            return result;
        }
    }
}
