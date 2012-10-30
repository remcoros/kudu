using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace HigLabo.Net
{
    public partial class HttpClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected Task<T> CreateNewTask<T>(Func<T> func)
        {
            return Task.Factory.StartNew<T>(func);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<HttpWebResponse> GetHttpWebResponseAsync(String url)
        {
            return this.CreateNewTask<HttpWebResponse>(() => this.GetHttpWebResponse(url));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<HttpWebResponse> GetHttpWebResponseAsync(String url, HttpBodyFormUrlEncodedData data)
        {
            return this.CreateNewTask<HttpWebResponse>(() => this.GetHttpWebResponse(url, data));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<HttpWebResponse> GetHttpWebResponseAsync(String url, Byte[] data)
        {
            return this.CreateNewTask<HttpWebResponse>(() => this.GetHttpWebResponse(url, data));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Task<HttpWebResponse> GetHttpWebResponseAsync(String url, Stream stream)
        {
            return this.CreateNewTask<HttpWebResponse>(() => this.GetHttpWebResponse(url, stream));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<HttpWebResponse> GetHttpWebResponseAsync(HttpRequestCommand command)
        {
            return this.CreateNewTask<HttpWebResponse>(() => this.GetHttpWebResponse(command));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<HttpResponse> GetResponseAsync(String url)
        {
            return this.CreateNewTask<HttpResponse>(() => this.GetResponse(url));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<HttpResponse> GetResponseAsync(String url, HttpBodyFormUrlEncodedData data)
        {
            return this.CreateNewTask<HttpResponse>(() => this.GetResponse(url, data));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<HttpResponse> GetResponseAsync(String url, Byte[] data)
        {
            return this.CreateNewTask<HttpResponse>(() => this.GetResponse(url, data));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Task<HttpResponse> GetResponseAsync(String url, Stream stream)
        {
            return this.CreateNewTask<HttpResponse>(() => this.GetResponse(url, stream));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<HttpResponse> GetResponseAsync(HttpRequestCommand command)
        {
            return this.CreateNewTask<HttpResponse>(() => this.GetResponse(command));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<String> GetBodyTextAsync(String url)
        {
            return this.CreateNewTask<String>(() => this.GetBodyText(url));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<String> GetBodyTextAsync(String url, HttpBodyFormUrlEncodedData data)
        {
            return this.CreateNewTask<String>(() => this.GetBodyText(url, data));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<String> GetBodyTextAsync(String url, Byte[] data)
        {
            return this.CreateNewTask<String>(() => this.GetBodyText(url, data));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Task<String> GetBodyTextAsync(String url, Stream stream)
        {
            return this.CreateNewTask<String>(() => this.GetBodyText(url, stream));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Task<String> GetBodyTextAsync(HttpRequestCommand command)
        {
            return this.CreateNewTask<String>(() => this.GetBodyText(command));
        }
    }
}
