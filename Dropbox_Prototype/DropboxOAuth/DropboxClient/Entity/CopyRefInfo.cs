using System;
using HigLabo.Net.Extensions;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class CopyRefInfo : ResponseObject
    {
        /// <summary>
        /// 
        /// </summary>
        public String Ref { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset Expires { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonText"></param>
        public CopyRefInfo(String jsonText)
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
            
            this.Ref = d.ToString("copy_ref");
            this.Expires = d.ToDateTimeOffset("expires") ?? DateTimeOffset.MinValue;
        }
    }
}
