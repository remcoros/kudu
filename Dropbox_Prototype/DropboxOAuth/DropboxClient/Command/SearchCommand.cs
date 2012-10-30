using System;
using System.Collections.Generic;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class SearchCommand : DropboxCommand
    {
        /// <summary>
        /// 
        /// </summary>
        public RootFolder Root { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Path { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Query { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Int32 FileLimit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boolean IncludeDeleted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SearchCommand()
        {
            Root = RootFolder.Dropbox;
            FileLimit = 1000;
            IncludeDeleted = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IDictionary<string, string> CreateParameters()
        {
            var d = new Dictionary<String, String>();
            d["query"] = this.Query;
            d["file_limit"] = this.FileLimit.ToString();
            d["include_deleted"] = this.IncludeDeleted.ToString().ToLower();
            return d;
        }
    }
}
