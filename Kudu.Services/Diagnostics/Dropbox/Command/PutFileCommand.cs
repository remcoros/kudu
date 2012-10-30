using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class PutFileCommand : DropboxCommand
    {        
        private Byte[] _fileData = new Byte[0];

        /// <summary>
        /// 
        /// </summary>
        public Boolean OverWrite { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RootFolder Root { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String ParentRev { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Path { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public PutFileCommand()
        {
            Root = RootFolder.Dropbox;
            OverWrite = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IDictionary<string, string> CreateParameters()
        {
            var d = new Dictionary<String, String>();
            d["overwrite"] = this.OverWrite.ToString().ToLower();
            d["parent_rev"] = this.ParentRev;
            return d;
        }
#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadFileData(String filePath)
        {
            _fileData = File.ReadAllBytes(filePath);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileInfo"></param>
        public void LoadFileData(FileInfo fileInfo)
        {
            Byte[] bb;
            using (var r = new BinaryReader(fileInfo.OpenRead(), Encoding.UTF8))
            {
                bb = new Byte[fileInfo.Length];
                r.Read(bb, 0, bb.Length);
            }
            _fileData = bb;
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void LoadFileData(Byte[] data)
        {
            _fileData = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Byte[] CreatePutData()
        {
            return _fileData;
        }
    }
}
