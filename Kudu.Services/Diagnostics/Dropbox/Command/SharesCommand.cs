using System;
using System.Collections.Generic;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class SharesCommand : DropboxCommand
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
        public SharesCommand()
        {
            Root = RootFolder.Dropbox;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IDictionary<string, string> CreateParameters()
        {
            var d = new Dictionary<String, String>();
            return d;
        }
    }
}
