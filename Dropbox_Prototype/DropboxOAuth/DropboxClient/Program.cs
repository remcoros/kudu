using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using HigLabo.Net.Dropbox;
using System.IO;

namespace dropboxclient
{
    class Program
    {
        const string ConsumerKey = "hbbdkdh8y0qigrn";
        const string ConsumerSecret = "to6vjhxko848lgx";
        const string Token = "w7fv41cg30gg1jo";
        const string TokenSecret = "ka9n4keicpztufu";
        static string Destination;
        static DropboxClient _client;

        static void Main(string[] args)
        {
            try
            {
                Destination = args[0];

                _client = new DropboxClient(ConsumerKey, ConsumerSecret, Token, TokenSecret);

                //GetAccountInfo();
                //ListAllDirectories();
                //GetDelta(args.Length >= 1 ? args[0] : null);
                SyncFolder();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static private void SyncFolder()
        {
            string cursorFile = Path.Combine(Destination, ".cursor");
            string cursor = File.ReadAllText(cursorFile);
            cursor = string.IsNullOrEmpty(cursor) ? null : cursor;
            var deltas = GetDelta(ref cursor);
            if (deltas == null)
            {
                Console.WriteLine("nothing changed!");
                return;
            }

            var src = GetServerDirectories();
            var dst = GetClientDirectories();
            foreach (KeyValuePair<string, Metadata> delta in deltas)
            {
                var path = delta.Key;
                var metadata = delta.Value;
                FileSystemInfo result;
                if (dst.TryGetValue(path, out result))
                {
                    if (metadata == null || string.IsNullOrEmpty(metadata.Path))
                    {
                        Delete(result);
                        dst.Remove(path);
                    }
                    else
                    {
                        Overwrite(metadata, ref result);
                        dst[GetPath(result)] = result;
                    }
                }
                else
                {
                    if (metadata != null && !string.IsNullOrEmpty(metadata.Path))
                    {
                        var info = Create(metadata);
                        dst[GetPath(info)] = info;
                    }
                }
            }

            File.WriteAllText(cursorFile, cursor);
        }

        static private FileSystemInfo Create(Metadata metadata)
        {
            string path = GetPath(metadata);
            if (metadata.IsDirectory)
            {
                if (!Directory.Exists(path))
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    Console.WriteLine("mkdir " + path);
                    Directory.CreateDirectory(path);
                }

                return new DirectoryInfo(path);
            }
            else
            {
                byte[] bytes = _client.GetFile(new GetFileCommand { Root = RootFolder.Sandbox, Path = metadata.Path });
                File.WriteAllBytes(path, bytes);
                Console.WriteLine("copy " +  metadata.Path + " " + path);
                return new FileInfo(path);
            }
        }

        static private void Delete(FileSystemInfo info)
        {
            if (info.Exists)
            {
                if (info is DirectoryInfo)
                {
                    ((DirectoryInfo)info).Delete(true);
                    Console.WriteLine("rmdir " + info.FullName);
                }
                else
                {
                    info.Delete();
                    Console.WriteLine("del " + info.FullName);
                }
            }
        }

        static private void Overwrite(Metadata metadata, ref FileSystemInfo info)
        {
            if (metadata.IsDirectory)
            {
                if (!(info is DirectoryInfo))
                {
                    info.Delete();
                    info = Directory.CreateDirectory(info.FullName);
                    Console.WriteLine("mkdir " + info.FullName);
                }
            }
            else
            {
                if (info is DirectoryInfo)
                {
                    info.Delete();
                }

                byte[] bytes = _client.GetFile(new GetFileCommand { Root = RootFolder.Sandbox, Path = metadata.Path });
                File.WriteAllBytes(info.FullName, bytes);
                Console.WriteLine("overwrite " + info.FullName);
                info = new FileInfo(info.FullName);
            }
        }

        static private Dictionary<string, FileSystemInfo> GetClientDirectories()
        {
            var dict = new Dictionary<string, FileSystemInfo>(StringComparer.OrdinalIgnoreCase);
            DirectoryInfo parent = new DirectoryInfo(Destination);
            dict.Add("/", parent);
            foreach (var info in parent.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                dict.Add(GetPath(info), info);
            }
            return dict;
        }

        static private string GetPath(FileSystemInfo info)
        {
            return info.FullName.Replace(Destination, string.Empty).Replace('\\', '/');
        }

        static private string GetPath(Metadata metadata)
        {
            return Path.Combine(Destination, metadata.Path.Trim('/').Replace('/', '\\'));
        }

        static private Dictionary<string, string> GetServerDirectories()
        {
            _client = new DropboxClient(ConsumerKey, ConsumerSecret, Token, TokenSecret);
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            GetAllDirectories(dict);
            return dict;
        }

        static private void GetAllDirectories(Dictionary<string, string> dict, string path = "/")
        {
            Metadata metadata = _client.GetMetadata(new GetMetadataCommand { Root = RootFolder.Sandbox, Path = path });
            dict.Add(metadata.Path, metadata.Path);
            foreach (Metadata child in metadata.Contents)
            {
                if (child.IsDirectory)
                {
                    GetAllDirectories(dict, child.Path);
                }
                else
                {
                    dict.Add(child.Path, child.Path);
                }
            }
        }

        static private Dictionary<string, Metadata> GetDelta(ref string cursor)
        {
            _client = new DropboxClient(ConsumerKey, ConsumerSecret, Token, TokenSecret);
            var delta = _client.GetDelta(new GetDeltaCommand { Cursor = cursor });
            if (delta.Cursor == cursor)
            {
                return null;
            }
            cursor = delta.Cursor;

            var dict = new Dictionary<string, Metadata>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in delta.Entries)
            {
                dict.Add(entry.Path, entry.Metadata);
            }
            return dict;
        }

        static private void ListAllDirectories()
        {
            _client = new DropboxClient(ConsumerKey, ConsumerSecret, Token, TokenSecret);
            //Metadata metadata = client.GetMetadata(new GetMetadataCommand { Path = "/apps/aspnet01" });
            //Console.WriteLine(metadata.Path);
            ListAllDirectories();
        }

        static private void ListAllDirectories(string path = "/")
        {
            Metadata metadata = _client.GetMetadata(new GetMetadataCommand { Root = RootFolder.Sandbox, Path = path });
            Console.WriteLine(metadata.Path);
            foreach (Metadata child in metadata.Contents)
            {
                if (child.IsDirectory)
                {
                    ListAllDirectories( child.Path);
                }
                else
                {
                    Console.WriteLine(child.Path);
                }
            }
        }

        static private void GetAccountInfo()
        {
            _client = new DropboxClient(ConsumerKey, ConsumerSecret, Token, TokenSecret);
            AccountInfo accountInfo = _client.GetAccountInfo();
            Console.WriteLine(accountInfo.DisplayName);
        }

        static private void GetAccountInfoRaw()
        {
            //GET https://api.dropbox.com/1/account/info
            HttpClient client = CreateClient();
            HttpResponseMessage response = client.GetAsync("/1/account/info").Result.EnsureSuccessStatusCode();
            string result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(result);
        }

        static HttpClient CreateClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.dropbox.com/");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth",
                string.Format("oauth_version=\"1.0\", oauth_signature_method=\"PLAINTEXT\", oauth_consumer_key=\"{0}\", oauth_token=\"{1}\", oauth_signature=\"{2}&{3}\"", ConsumerKey, Token, ConsumerSecret, TokenSecret));
            return client;
        }
    }
}