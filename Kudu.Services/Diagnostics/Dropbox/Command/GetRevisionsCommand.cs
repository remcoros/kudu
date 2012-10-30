using System;
using System.Collections.Generic;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class GetRevisionsCommand : DropboxCommand
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
        public Int32 RevLimit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GetRevisionsCommand()
        {
            Root = RootFolder.Dropbox;
            RevLimit = 10;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IDictionary<string, string> CreateParameters()
        {
            var d = new Dictionary<String, String>();
            d["rev_limit"] = this.RevLimit.ToString();
            return d;
        }
    }
}
