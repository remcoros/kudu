using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HigLabo.Net.Extensions;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class DeltaInfo : ResponseObject
    {
        /// <summary>
        /// 
        /// </summary>
        public Boolean Reset { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Cursor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boolean HasMore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EntryInfo> Entries { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonText"></param>
        public DeltaInfo(String jsonText)
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

            this.Reset = d.ToBoolean("reset") ?? false;
            this.Cursor = d.ToString("cursor");
            this.HasMore = d.ToBoolean("has_more") ?? false;

            Entries = new List<EntryInfo>();
            var ja = JsonConvert.DeserializeObject(d.ToString("entries")) as JArray;
            foreach (var i in ja)
            {
                Entries.Add(new EntryInfo(i.ToString()));    
            }
        }
    }
}
