using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace HigLabo.Net.Dropbox
{
    public partial class DropboxClient
    {
        private String GetResponseBodyText(String url, DropboxCommand command)
        {
            return this.GetResponseBodyText(HttpMethodName.Get, url, command);
        }
        private String GetResponseBodyText(HttpMethodName methodName, String url, DropboxCommand command)
        {
            var u = _protocol.ToString().ToLower() + "://" + url;
            IDictionary<String, String> d = new Dictionary<String, String>();

            if (methodName == HttpMethodName.Put ||
                methodName == HttpMethodName.Delete)
            { throw new ArgumentException(); }

            if (command != null)
            {
                d = command.GetParameters();
            }
            var res = this.GetResponse(methodName, u, this._accessToken, this._accessTokenSecret, d);
            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpResponseException(res);
            }
            return res.BodyText;
        }
        private Stream GetHttpWebResponse(HttpMethodName methodName, String url, DropboxCommand command)
        {
            String u = _protocol.ToString().ToLower() + "://" + url;
            IDictionary<String, String> d = new Dictionary<string, string>();

            if (methodName == HttpMethodName.Put ||
                methodName == HttpMethodName.Delete)
            { throw new ArgumentException(); }

            if (command != null)
            {
                d = command.GetParameters();
            }
            var rs = this.GetHttpWebResponse(methodName, u, this._accessToken, this._accessTokenSecret, d);
            return rs.GetResponseStream();
        }
        private String Post(String url, UploadFileCommand command)
        {
            var u = _protocol.ToString().ToLower() + "://" + url;
            var d = command.GetParameters();
            d["file"] = command.FileName;
            var cm = this.CreateHttpRequestCommand(HttpMethodName.Post, u, this.AccessToken, this.AccessTokenSecret, d);
            var boundary = UploadFileCommand.GetBoundaryString();
            cm.ContentType = "multipart/form-data; boundary=\"" + boundary + "\"";
            cm.SetBodyStream(command.CreatePostData(boundary));

            return this.GetResponse(cm).BodyText;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumerKey"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static AccessTokenInfo GetAccessTokenInfo(String consumerKey, String email, String password)
        {
            var cl = new HttpClient();
            var url = String.Format("https://{0}?oauth_consumer_key={1}&email={2}&password={3}"
                , DropboxApiUrl.Version1.Token, consumerKey, email, password);
            var d = JsonConvert.DeserializeObject<Dictionary<String, Object>>(cl.GetBodyText(url));
            return new AccessTokenInfo(d["token"].ToString(), d["secret"].ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AccountInfo GetAccountInfo()
        {
            return this.GetAccountInfo(null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AccountInfo GetAccountInfo(GetAccountCommand command)
        {
            var text = this.GetResponseBodyText(DropboxApiUrl.Version1.GetAccount, command);
            return new AccountInfo(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public String CreateAccount(CreateAccountCommand command)
        {
            return this.GetResponseBodyText(DropboxApiUrl.Version1.CreateAccount, command);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Metadata GetMetadata(GetMetadataCommand command)
        {
            var text = this.GetResponseBodyText(DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Metadata
                                                                        , command.Root.ToString()
                                                                        , command.Path)
                                                , command);
            return new Metadata(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Byte[] GetFile(String path)
        {
            return this.GetFile(path, RootFolder.Dropbox);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public Byte[] GetFile(String path, RootFolder root)
        {
            return GetFile(new GetFileCommand { Path = path, Root = root });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public Byte[] GetFile(GetFileCommand cmd)
        {
            if (String.IsNullOrEmpty(cmd.Path)) { throw new ArgumentException(); }
            var str = this.GetHttpWebResponse(HttpMethodName.Get
                                                 , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Files, cmd.Root.ToString(), cmd.Path)
                                                 , cmd);
            return str.ToByteArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public Metadata UploadFile(UploadFileCommand command)
        {
            if (String.IsNullOrEmpty(command.FolderPath)) { throw new ArgumentException(); }
            var url = DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Files, command.Root.ToString(), command.FolderPath);
            return new Metadata(this.Post(url, command));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Byte[] GetThumbnails(GetThumbnailsCommand command)
        {
            var str = this.GetHttpWebResponse(HttpMethodName.Get
                                , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Thumbnail, command.Root.ToString(), command.Path)
                                , command);
            return str.ToByteArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Metadata CopyFile(MoveFileCommand command)
        {
            var text = this.GetResponseBodyText(HttpMethodName.Post, DropboxApiUrl.Version1.CopyFile, command);
            return new Metadata(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Metadata MoveFile(MoveFileCommand command)
        {
            var text = this.GetResponseBodyText(HttpMethodName.Post, DropboxApiUrl.Version1.MoveFile, command);
            return new Metadata(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Metadata CreateFolder(CreateFolderCommand command)
        {
            var text = this.GetResponseBodyText(HttpMethodName.Post, DropboxApiUrl.Version1.CreateFolder, command);
            return new Metadata(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Metadata Delete(DeleteCommand command)
        {
            var text = this.GetResponseBodyText(HttpMethodName.Post, DropboxApiUrl.Version1.Delete, command);
            return new Metadata(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Metadata PutFile(PutFileCommand command)
        {
            if (String.IsNullOrEmpty(command.Path)) { throw new ArgumentException(); }
            var url = DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.FilesPut, command.Root.ToString(), command.Path);
            url = _protocol.ToString().ToLower() + "://" + url;
            var cm = this.CreateHttpRequestCommand(HttpMethodName.Put, url, this.AccessToken, this.AccessTokenSecret, command.GetParameters());
            cm.SetBodyStream(command.CreatePutData());
            return new Metadata(this.GetResponse(cm).BodyText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public List<Metadata> GetRevisions(GetRevisionsCommand cmd)
        {
            var text = this.GetResponseBodyText(HttpMethodName.Get
                    , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Revision, cmd.Root.ToString(), cmd.Path)
                    , cmd);
            return GetMetadataList(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public List<Metadata> Search(SearchCommand cmd)
        {
            var text = this.GetResponseBodyText(HttpMethodName.Post
                    , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Search, cmd.Root.ToString(), cmd.Path)
                    , cmd);
            return GetMetadataList(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public Metadata Restore(RestoreCommand cmd)
        {
            var text = this.GetResponseBodyText(HttpMethodName.Post
                , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Restore, cmd.Root.ToString(), cmd.Path)
                , cmd);
            return new Metadata(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public LinkInfo GetShareableLink(SharesCommand cmd)
        {
            var text = this.GetResponseBodyText(HttpMethodName.Post
                , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Share, cmd.Root.ToString(), cmd.Path)
                , cmd);
            return new LinkInfo(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public LinkInfo GetMediaLink(MediaCommand cmd)
        {
            var text = this.GetResponseBodyText(HttpMethodName.Post
                , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Media, cmd.Root.ToString(), cmd.Path)
                , cmd);
            return new LinkInfo(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public CopyRefInfo GetCopyRef(GetCopyRefCommand cmd)
        {
            var text = this.GetResponseBodyText(HttpMethodName.Get
                , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.CopyRef, cmd.Root.ToString(), cmd.Path)
                , cmd);
            return new CopyRefInfo(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public DeltaInfo GetDelta(GetDeltaCommand cmd)
        {
            var text = this.GetResponseBodyText(HttpMethodName.Post
                , DropboxApiUrl.GetApiUrl(DropboxApiUrl.Version1.Delta, "")
                , cmd);
            return new DeltaInfo(text);
        }
    }
}
