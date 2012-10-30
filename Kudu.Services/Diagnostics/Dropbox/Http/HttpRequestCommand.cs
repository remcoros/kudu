using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
#if !NETFX_CORE
using System.Security.Cryptography.X509Certificates;
#endif

namespace HigLabo.Net
{
    /// <summary>
    /// 
    /// </summary>
#if !SILVERLIGHT && !NETFX_CORE
    [Serializable]
#endif
    public class HttpRequestCommand
    {
        private WebHeaderCollection _Headers = new WebHeaderCollection();
        /// <summary>
        /// 
        /// </summary>
        public String Url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public HttpMethodName MethodName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsSendBodyStream { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public WebHeaderCollection Headers
        {
            get { return _Headers; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String ContentType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Stream BodyStream { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public Func<String, String> UrlEncodeFunction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public HttpRequestCommand(String url)
        {
            this.InitializeProperty();
            this.Url = url;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="methodName"></param>
        public HttpRequestCommand(String url, HttpMethodName methodName)
        {
            this.InitializeProperty();
            this.Url = url;
            this.MethodName = methodName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="stream"></param>
        public HttpRequestCommand(String url, Stream stream)
        {
            this.InitializeProperty();
            this.Url = url;
            this.MethodName = HttpMethodName.Post;
            this.BodyStream = stream;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        public HttpRequestCommand(String url, HttpBodyFormUrlEncodedData data)
        {
            this.InitializeProperty();
            this.Url = url;
            this.MethodName = HttpMethodName.Post;
            this.ContentType = HttpClient.ApplicationFormUrlEncoded;
            this.BodyStream = new MemoryStream(this.CreateRequestBodyData(data.Encoding, data.Values));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        public HttpRequestCommand(String url, Byte[] data)
        {
            this.InitializeProperty();
            this.Url = url;
            this.MethodName = HttpMethodName.Post;
            this.BodyStream = new MemoryStream(data);
        }
        private void InitializeProperty()
        {
            this.UrlEncodeFunction = HttpClient.UrlEncode;
            this.MethodName = HttpMethodName.Get;
            this.IsSendBodyStream = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public Byte[] CreateRequestBodyData(Encoding encoding, Dictionary<String, String> values)
        {
            StringBuilder sb = new StringBuilder(512);
            var d = values;
            if (d == null || d.Keys.Count == 0) { return new Byte[0]; }

            foreach (var key in d.Keys)
            {
                sb.AppendFormat("{0}={1}&", this.UrlEncodeFunction(key), this.UrlEncodeFunction(d[key]));
            }
            return encoding.GetBytes(sb.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SetBodyStream(HttpBodyFormUrlEncodedData data)
        {
            if (String.IsNullOrEmpty(this.ContentType) == true)
            {
                this.ContentType = HttpClient.ApplicationFormUrlEncoded;
            }
            var bb = this.CreateRequestBodyData(data.Encoding, data.Values);
            this.BodyStream = new MemoryStream(bb);
            this.IsSendBodyStream = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SetBodyStream(Byte[] data)
        {
            this.BodyStream = new MemoryStream(data);
            this.IsSendBodyStream = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void SetBodyStream(Stream stream)
        {
            this.BodyStream = stream;
            this.IsSendBodyStream = true;
        }
    }
}
