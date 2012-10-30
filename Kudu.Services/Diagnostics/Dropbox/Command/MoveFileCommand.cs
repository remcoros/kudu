using System;
using System.Collections.Generic;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class MoveFileCommand : DropboxCommand
    {
        /// <summary>
        /// 
        /// </summary>
        public RootFolder Root { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String FromPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String ToPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MoveFileCommand()
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
            d["root"] = this.Root.ToString().ToLower();
            d["from_path"] = this.FromPath;
            d["to_path"] = this.ToPath;
            return d;
        }
    }
}
