using Kudu.Contracts.Tracing;
using Kudu.Core.Infrastructure;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Kudu.Core.Deployment
{
    public abstract class GeneratorSiteBuilder : ExternalCommandBuilder
    {
        public GeneratorSiteBuilder(IEnvironment environment, IBuildPropertyProvider propertyProvider, string sourcePath)
            : base(environment, sourcePath, propertyProvider)
        {
        }

        // TODO: Do we still need this
        /*protected string GetPropertyString()
        {
            return String.Join(";", _propertyProvider.GetProperties().Select(p => String.Format("{0}=\"{1}\"", p.Key, p.Value)));
        }*/

        public override Task Build(DeploymentContext context)
        {
            var tcs = new TaskCompletionSource<object>();

            // TODO: Localization for the default script
            ILogger buildLogger = context.Logger.Log(Resources.Log_BuildingWebProject, Path.GetFileName(RepositoryPath));

            try
            {
                GenerateScript(context, buildLogger);
                RunCommand(context, "deploy.cmd");
                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                buildLogger.Log(ex);
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        protected abstract string ScriptGeneratorCommandArguments { get; }

        private void GenerateScript(DeploymentContext context, ILogger buildLogger)
        {
            try
            {
                using (context.Tracer.Step("Generating deployment script"))
                {
                    var scriptGenerator = new Executable("node", RepositoryPath);

                    using (var writer = new ProgressWriter())
                    {
                        writer.Start();

                        // The line with the MSB3644 warnings since it's not important
                        string log = scriptGenerator.Execute(context.Tracer,
                                                   output =>
                                                   {
                                                       if (output.Contains("MSB3644:") || output.Contains("MSB3270:"))
                                                       {
                                                           return false;
                                                       }

                                                       writer.WriteOutLine(output);
                                                       return true;
                                                   },
                                                   error =>
                                                   {
                                                       writer.WriteErrorLine(error);
                                                       return true;
                                                   },
                                                   Console.OutputEncoding,
                                                   ScriptGeneratorCommandArguments).Item1;

                        buildLogger.Log(log);
                    }
                }
            }
            catch (Exception ex)
            {
                context.Tracer.TraceError(ex);

                // HACK: Log an empty error to the global logger (post receive hook console output).
                // The reason we don't log the real exception is because the 'live output' running
                // msbuild has already been captured.
                context.GlobalLogger.LogError();
            }
        }
    }
}
