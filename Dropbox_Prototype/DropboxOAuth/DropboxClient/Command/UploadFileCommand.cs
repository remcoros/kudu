using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class UploadFileCommand : DropboxCommand
    {
        private String _folderPath = "/";
        private Byte[] _fileData = new Byte[0];
        private String _contentType = "application/octet-stream";
        private String _fileName = "";

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
        public String FolderPath
        {
            get { return _folderPath; }
            set { _folderPath = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public UploadFileCommand()
        {
            // edit start
            Root = RootFolder.Dropbox;
            OverWrite = true;
            // edit end 
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static String GetBoundaryString()
        {
            return "UploadFileCommand_" + DateTime.Now.ToString("yyyyMMddHHmmss");
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
        public Byte[] CreatePostData()
        {
            var boundary = GetBoundaryString();
            return this.CreatePostData(boundary);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="boundary"></param>
        /// <returns></returns>
        public Byte[] CreatePostData(String boundary)
        {
            HttpBodyMultipartFormData data = new HttpBodyMultipartFormData();
            data.Encoding = Encoding.UTF8;
            data.Boundary = boundary;

            HttpBodyFormData fd = new HttpBodyFormData("file");
            fd.FileName = this.FileName;
            fd.ContentType = this.ContentType;
            fd.ContentTransferEncoding = "binary";
            fd.SetData(_fileData);
            data.FormDataList.Add(fd);

            return data.CreateBodyData();
        }
    }
}
