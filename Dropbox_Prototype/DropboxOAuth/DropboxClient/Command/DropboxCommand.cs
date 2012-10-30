using System;
using System.Collections.Generic;
using System.Linq;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DropboxCommand
    {
        /// <summary>
        /// 
        /// </summary>
        public String Locale { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDictionary<String, String> GetParameters()
        {
            var d = this.CreateParameters();
            if (!String.IsNullOrWhiteSpace(Locale))
                d.Add("locale", this.Locale);
            this.RemoveEmptyEntry(d);
            return d;
        }
        private void RemoveEmptyEntry(IDictionary<String, String> data)
        {
            var keys = data.Keys.Where(key => String.IsNullOrEmpty(data[key])).ToList();

            foreach (var t in keys)
            {
                data.Remove(t);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract IDictionary<String, String> CreateParameters();
    }
}
