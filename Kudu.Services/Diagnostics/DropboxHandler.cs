using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Kudu.Contracts.Infrastructure;
using Kudu.Contracts.Settings;
using Kudu.Contracts.SourceControl;
using Kudu.Contracts.Tracing;
using Kudu.Core;
using Kudu.Core.Deployment;
using Kudu.Core.Infrastructure;
using Kudu.Core.SourceControl.Git;
using Newtonsoft.Json.Linq;
using Kudu.Services.GitServer;
using System.Threading;
using HigLabo.Net.Dropbox;

namespace Kudu.Services.Performance
{
    public class DropboxHandler : GitServerHttpHandler
    {
        const string ConsumerKey = "hbbdkdh8y0qigrn";
        const string ConsumerSecret = "to6vjhxko848lgx";
        const string Token = "w7fv41cg30gg1jo";
        const string TokenSecret = "ka9n4keicpztufu";

        private readonly IDeploymentSettingsManager _settings;
        private readonly RepositoryConfiguration _configuration;
        private readonly IEnvironment _environment;
        private readonly DropboxClient _dropbox;

        public DropboxHandler(ITracer tracer,
                              IGitServer gitServer,
                              IDeploymentManager deploymentManager,
                              IDeploymentSettingsManager settings,
                              IOperationLock deploymentLock,
                              RepositoryConfiguration configuration,
                              IEnvironment environment)
            : base(tracer, gitServer, deploymentLock, deploymentManager)
        {
            _settings = settings;
            _configuration = configuration;
            _environment = environment;
            _dropbox = new DropboxClient(ConsumerKey, ConsumerSecret, Token, TokenSecret);
        }

        public override void ProcessRequest(HttpContext context)
        {
            using (_tracer.Step("DropboxHandler.ProcessRequest"))
            {
                context.Response.Buffer = false;
                context.Response.BufferOutput = false;
                context.Response.ContentType = "text/plain";

                _deploymentLock.LockOperation(() =>
                {
                    PerformDeployment(context);
                },
                () =>
                {
                    context.Response.StatusCode = 409;
                    context.ApplicationInstance.CompleteRequest();
                });
            }
        }

        private void PerformDeployment(HttpContext context)
        {
            using (_deploymentManager.CreateTemporaryDeployment(Resources.FetchingChanges))
            {
                 // Configure the repository
                _gitServer.Initialize(_configuration);

                //// Setup the receive info (this is important to know if branches were deleted etc)
                //_gitServer.SetReceiveInfo(repositoryInfo.OldRef, repositoryInfo.NewRef, targetBranch);

                // git checkout master --force
                _gitServer.Update();

                try
                {
                    // Sync with dropbox
                    context.Response.Write("Sync with Dropbox...\r\n");
                    if (SyncFolder(context))
                    {
                        // Add and commit
                        context.Response.Write("Commit changes...\r\n");
                        _gitServer.Commit("Sync Dropbox at " + DateTime.Now.ToString("g"), "Dropbox");

                        // Perform the actual deployment
                        context.Response.Write("Deploy...\r\n");
                        _deploymentManager.Deploy("Dropbox");

                        context.Response.Write("Done successfully!\r\n");
                    }
                    else
                    {
                        context.Response.Write("Nothing changed\r\n");
                    }
                }
                finally
                {
                    // git checkout master --force
                    _gitServer.Update();
                }
            }
        }

        private bool SyncFolder(HttpContext context)
        {
            string cursorFile = Path.Combine(_environment.RepositoryPath, ".cursor");
            string cursor = File.ReadAllText(cursorFile);
            cursor = string.IsNullOrEmpty(cursor) ? null : cursor;
            var deltas = GetDelta(ref cursor);
            if (deltas == null)
            {
                return false;
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
            return true;
        }

        private FileSystemInfo Create(Metadata metadata)
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

                    Directory.CreateDirectory(path);
                }

                return new DirectoryInfo(path);
            }
            else
            {
                byte[] bytes = _dropbox.GetFile(new GetFileCommand { Root = RootFolder.Sandbox, Path = metadata.Path });
                File.WriteAllBytes(path, bytes);
                return new FileInfo(path);
            }
        }

        private void Delete(FileSystemInfo info)
        {
            if (info.Exists)
            {
                if (info is DirectoryInfo)
                {
                    ((DirectoryInfo)info).Delete(true);
                }
                else
                {
                    info.Delete();
                }
            }
        }

        private void Overwrite(Metadata metadata, ref FileSystemInfo info)
        {
            if (metadata.IsDirectory)
            {
                if (!(info is DirectoryInfo))
                {
                    info.Delete();
                    info = Directory.CreateDirectory(info.FullName);
                }
            }
            else
            {
                if (info is DirectoryInfo)
                {
                    info.Delete();
                }

                byte[] bytes = _dropbox.GetFile(new GetFileCommand { Root = RootFolder.Sandbox, Path = metadata.Path });
                File.WriteAllBytes(info.FullName, bytes);
                info = new FileInfo(info.FullName);
            }
        }

        private Dictionary<string, FileSystemInfo> GetClientDirectories()
        {
            var dict = new Dictionary<string, FileSystemInfo>(StringComparer.OrdinalIgnoreCase);
            DirectoryInfo parent = new DirectoryInfo(_environment.RepositoryPath);
            dict.Add("/", parent);
            foreach (var info in parent.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                dict.Add(GetPath(info), info);
            }
            return dict;
        }

        private string GetPath(FileSystemInfo info)
        {
            return info.FullName.Replace(_environment.RepositoryPath, string.Empty).Replace('\\', '/');
        }

        private string GetPath(Metadata metadata)
        {
            return Path.Combine(_environment.RepositoryPath, metadata.Path.Trim('/').Replace('/', '\\'));
        }

        private Dictionary<string, string> GetServerDirectories()
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            GetAllDirectories(dict);
            return dict;
        }

        private void GetAllDirectories(Dictionary<string, string> dict, string path = "/")
        {
            Metadata metadata = _dropbox.GetMetadata(new GetMetadataCommand { Root = RootFolder.Sandbox, Path = path });
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

        private Dictionary<string, Metadata> GetDelta(ref string cursor)
        {
            var delta = _dropbox.GetDelta(new GetDeltaCommand { Cursor = cursor });
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
    }
}
