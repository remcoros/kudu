using System;
using System.Collections.Generic;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class GetMetadataCommand : DropboxCommand
    {
        private String _path = "/";
        private Int32 _fileLimit = 10000;
        private Boolean _list = true;

        /// <summary>
        /// 
        /// </summary>
        public RootFolder Root { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Hash { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boolean IncludeDeleted { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Rev { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Path
        {
            get { return _path; }
            set { _path = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Int32 FileLimit
        {
            get { return _fileLimit; }
            set { _fileLimit = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Boolean List
        {
            get { return _list; }
            set { _list = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public GetMetadataCommand()
        {
            this.Root = RootFolder.Dropbox;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IDictionary<string, string> CreateParameters()
        {
            var d = new Dictionary<String, String>();
            d["file_limit"] = this.FileLimit.ToString();
            d["list"] = this.List.ToString().ToLower();
            d["hash"] = this.Hash;
            d["include_deleted"] = this.IncludeDeleted.ToString().ToLower();
            d["rev"] = this.Rev;
            return d;
        }
    }
}
