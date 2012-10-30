using System;
using HigLabo.Net.Extensions;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class LinkInfo : ResponseObject
    {
        /// <summary>
        /// 
        /// </summary>
        public String Url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset Expires { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonText"></param>
        public LinkInfo(String jsonText)
        {
            this.SetProperty(jsonText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonText"></param>
        public override void SetProperty(String jsonText)
        {
            var d = this.SetData(jsonText);
            
            this.Url = d.ToString("url");
            this.Expires = d.ToDateTimeOffset("expires") ?? DateTimeOffset.MinValue;
        }
    }
}
