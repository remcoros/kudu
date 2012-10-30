using System;
using System.Collections.Generic;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class CopyFileCommand : DropboxCommand
    {
        private RootFolder _root = RootFolder.Dropbox;
        private String _fromPath = "";
        private String _toPath = "";

        /// <summary>
        /// 
        /// </summary>
        public String FromCopyRef { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RootFolder Root
        {
            get { return _root; }
            set { _root = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String FromPath
        {
            get { return _fromPath; }
            set { _fromPath = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String ToPath
        {
            get { return _toPath; }
            set { _toPath = value; }
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
            d["from_copy_ref"] = this.FromCopyRef;
            return d;
        }
    }
}
