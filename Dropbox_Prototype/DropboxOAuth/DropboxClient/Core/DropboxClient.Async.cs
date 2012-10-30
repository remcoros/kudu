using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HigLabo.Net.Dropbox
{
    public partial class DropboxClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected new static Task<T> CreateNewTask<T>(Func<T> func)
        {
            return Task.Factory.StartNew<T>(func);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumerKey"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Task<AccessTokenInfo> GetAccessTokenInfoAsync(String consumerKey, String email, String password)
        {
            return CreateNewTask<AccessTokenInfo>(() => GetAccessTokenInfo(consumerKey, email, password));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<AccountInfo> GetAccountInfoAsync()
        {
            return CreateNewTask<AccountInfo>(() => GetAccountInfo());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<AccountInfo> GetAccountInfoAsync(GetAccountCommand command)
        {
            return CreateNewTask<AccountInfo>(() => GetAccountInfo(command));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<String> CreateAccountAsync(CreateAccountCommand command)
        {
            return CreateNewTask<String>(() => CreateAccount(command));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<Metadata> GetMetadataAsync(GetMetadataCommand command)
        {
            return CreateNewTask<Metadata>(() => GetMetadata(command));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Task<Byte[]> GetFileAsync(String path)
        {
            return CreateNewTask<Byte[]>(() => GetFile(path));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public Task<Byte[]> GetFileAsync(String path, RootFolder root)
        {
            return CreateNewTask<Byte[]>(() => GetFile(path, root));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public Task<Byte[]> GetFileAsync(GetFileCommand cmd)
        {
            return CreateNewTask<Byte[]>(() => GetFile(cmd));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public Task<Metadata> UploadFileAsync(UploadFileCommand command)
        {
            return CreateNewTask<Metadata>(() => UploadFile(command));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<Byte[]> GetThumbnailsAsync(GetThumbnailsCommand command)
        {
            return CreateNewTask<Byte[]>(() => GetThumbnails(command));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<Metadata> CopyFileAsync(MoveFileCommand command)
        {
            return CreateNewTask<Metadata>(() => CopyFile(command));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<Metadata> MoveFileAsync(MoveFileCommand command)
        {
            return CreateNewTask<Metadata>(() => MoveFile(command));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<Metadata> CreateFolderAsync(CreateFolderCommand command)
        {
            return CreateNewTask<Metadata>(() => CreateFolder(command));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<Metadata> DeleteAsync(DeleteCommand command)
        {
            return CreateNewTask<Metadata>(() => Delete(command));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<Metadata> PutFileAsync(PutFileCommand command)
        {
            return CreateNewTask<Metadata>(() => PutFile(command));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public Task<List<Metadata>> GetRevisionsAsync(GetRevisionsCommand cmd)
        {
            return CreateNewTask<List<Metadata>>(() => GetRevisions(cmd));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public Task<List<Metadata>> SearchAsync(SearchCommand cmd)
        {
            return CreateNewTask<List<Metadata>>(() => Search(cmd));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public Task<Metadata> RestoreAsync(RestoreCommand cmd)
        {
            return CreateNewTask<Metadata>(() => Restore(cmd));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public Task<LinkInfo> GetShareableLinkAsync(SharesCommand cmd)
        {
            return CreateNewTask<LinkInfo>(() => GetShareableLink(cmd));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public Task<LinkInfo> GetMediaLinkAsync(MediaCommand cmd)
        {
            return CreateNewTask<LinkInfo>(() => GetMediaLink(cmd));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public Task<CopyRefInfo> GetCopyRefAsync(GetCopyRefCommand cmd)
        {
            return CreateNewTask<CopyRefInfo>(() => GetCopyRef(cmd));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public Task<DeltaInfo> GetDeltaAsync(GetDeltaCommand cmd)
        {
            return CreateNewTask<DeltaInfo>(() => GetDelta(cmd));
        }
    }
}
