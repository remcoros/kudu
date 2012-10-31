using Kudu.Contracts.Tracing;
using Kudu.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Kudu.Core.Deployment
{
    public class WapBuilder : GeneratorSiteBuilder
    {
        private readonly string _projectPath;
        private readonly string _solutionPath;

        public WapBuilder(IEnvironment environment, IBuildPropertyProvider propertyProvider, string sourcePath, string projectPath, string solutionPath)
            : base(environment, propertyProvider, sourcePath)
        {
            _projectPath = projectPath;
            _solutionPath = solutionPath;
        }

        protected override string ScriptGeneratorCommandArguments
        {
            get
            {
                string arguments = "scriptGenerator.js --repositoryRoot=\"{0}\" --projectType=\"{1}\" --projectFile=\"{2}\"";
                List<string> args = new List<string>() { RepositoryPath, ProjectType.Wap.ToString(), _projectPath };

                if (!string.IsNullOrWhiteSpace(_solutionPath))
                {
                    arguments += " --solutionFile=\"{3}\"";
                    args.Add(_solutionPath);
                }
            }
        }
    }
}
