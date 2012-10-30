using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class EntryInfo : ResponseObject
    {
        /// <summary>
        /// 
        /// </summary>
        public String Path { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Metadata Metadata { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonText"></param>
        public EntryInfo(String jsonText)
        {
            this.SetProperty(jsonText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonText"></param>
        public override void SetProperty(String jsonText)
        {
            var ja = JsonConvert.DeserializeObject(jsonText) as JArray;
            if (ja.Count >= 1)
            {
                Path = ja[0].ToString().Replace("\"", "");
            }
            if (ja.Count == 2)
            {
                Metadata = new Metadata(ja[1].ToString());
            }
        }
    }
}
