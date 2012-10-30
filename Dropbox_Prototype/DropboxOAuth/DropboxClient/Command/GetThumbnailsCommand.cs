using System;
using System.Collections.Generic;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class GetThumbnailsCommand : DropboxCommand
    {
        /// <summary>
        /// 
        /// </summary>
        public enum ThumbnailFormat
        {
            /// <summary>
            /// 
            /// </summary>
            JPEG,
            /// <summary>
            /// 
            /// </summary>
            PNG
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ThumbnailSize
        {
            // edit start 
            /// <summary>
            /// 
            /// </summary>
            L,
            /// <summary>
            /// 
            /// </summary>
            XL,
            // edit end
            /// <summary>
            /// 
            /// </summary>
            Small, 
            /// <summary>
            /// 
            /// </summary>
            Medium, 
            /// <summary>
            /// 
            /// </summary>
            Large
        }

        private String _path = "/";
        private ThumbnailSize _size = ThumbnailSize.Small;
        private ThumbnailFormat _format = ThumbnailFormat.JPEG;

        /// <summary>
        /// 
        /// </summary>
        public RootFolder Root { get; set; }
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
        public ThumbnailFormat Format
        {
            get { return _format; }
            set { _format = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public ThumbnailSize Size
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IDictionary<string, string> CreateParameters()
        {
            var d = new Dictionary<String, String>();
            d["format"] = this.Format.ToString().ToLower();
            d["size"] = this.Size.ToString().ToLower();
            return d;
        }
    }
}
