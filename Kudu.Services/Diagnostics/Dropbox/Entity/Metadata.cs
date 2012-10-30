using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using HigLabo.Net.Extensions;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class Metadata : ResponseObject
    {
        private DateTimeOffset _modified = DateTimeOffset.MinValue;
        private String _path = "";
        private String _hash = "";
        private String _revision = "";
        private String _size = "";
        private String _root = "";
        private String _icon = "";
        private readonly List<Metadata> _contents = new List<Metadata>();

        /// <summary>
        /// 
        /// </summary>
        public String MimeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Rev { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsDeleted { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset Clientmtime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ThumbExists { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Bytes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset Modified
        {
            get { return _modified; }
            set { _modified = value; }
        }
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
        public String Hash
        {
            get { return _hash; }
            set { _hash = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String Revision
        {
            get { return _revision; }
            set { _revision = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDirectory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Size
        {
            get { return _size; }
            set { _size = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String Root
        {
            get { return _root; }
            set { _root = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public List<Metadata> Contents
        {
            get { return _contents; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Metadata()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonText"></param>
        public Metadata(String jsonText)
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
            this.ThumbExists = d.ToBoolean("thumb_exists") ?? this.ThumbExists;
            this.Bytes = d.ToInt32("bytes") ?? this.Bytes;
            this.Modified = d.ToDateTimeOffset("modified") ?? this.Modified;
            this.Path = d.ToString("path");
            this.Hash = d.ToString("hash");
            this.Revision = d.ToString("revision");
            this.IsDirectory = d.ToBoolean("is_dir") ?? this.IsDirectory;
            this.Size = d.ToString("size");
            this.Root = d.ToString("root");
            this.Icon = d.ToString("icon");
            this.MimeType = d.ToString("mime_type");
            this.Clientmtime = d.ToDateTimeOffset("client_mtime") ?? DateTimeOffset.MinValue;
            this.Rev = d.ToString("rev");
            this.IsDeleted = d.ToBoolean("is_deleted") ?? false;

            if (d.ContainsKey("contents") == true)
            {
                foreach (var rs in d["contents"] as JContainer)
                {
                    _contents.Add(new Metadata(rs.ToString()));
                }
            }
        }
    }
}
