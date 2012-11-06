﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kudu.Contracts.Infrastructure;
using Kudu.Contracts.Tracing;
using Kudu.Core.Deployment;
using Kudu.Services.Infrastructure;
using Newtonsoft.Json.Linq;

namespace Kudu.Services.Deployment
{
    public class DeploymentController : ApiController
    {
        private readonly IDeploymentManager _deploymentManager;
        private readonly ITracer _tracer;
        private readonly IOperationLock _deploymentLock;

        public DeploymentController(ITracer tracer,
                                    IDeploymentManager deploymentManager,
                                    IOperationLock deploymentLock)
        {
            _tracer = tracer;
            _deploymentManager = deploymentManager;
            _deploymentLock = deploymentLock;
        }

        /// <summary>
        /// Delete a deployment
        /// </summary>
        /// <param name="id">id of the deployment to delete</param>
        [HttpDelete]
        public void Delete(string id)
        {
            using (_tracer.Step("DeploymentService.Delete"))
            {
                _deploymentLock.LockHttpOperation(() =>
                {
                    try
                    {
                        _deploymentManager.Delete(id);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex));
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Conflict, ex));
                    }
                });
            }
        }

        /// <summary>
        /// Deploy a previous deployment
        /// </summary>
        /// <param name="id">id of the deployment to redeploy</param>
        [HttpPut]
        public void Deploy(string id, JObject input)
        {
            JObject result = GetJsonContent();

            // Just block here to read the json payload from the body
            using (_tracer.Step("DeploymentService.Deploy(id)"))
            {
                _deploymentLock.LockHttpOperation(() =>
                {
                    try
                    {
                        bool clean = false;

                        if (result != null)
                        {
                            clean = result.Value<bool>("clean");
                        }

                        string username = null;
                        if (input != null)
                        {
                            username = input.Value<string>("deployer");
                        }

                        if (username == null)
                        {
                            AuthUtility.TryExtractBasicAuthUser(Request, out username);
                        }

                        if (id == null)
                        {
                            // Null ID means deploy the latest commit on the current branch

                            _deploymentManager.Deploy(username);
                        }
                        else
                        {
                            _deploymentManager.Deploy(id, username, clean);
                        }
                    }
                    catch (FileNotFoundException ex)
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex));
                    }
                });
            }
        }
        
        /// <summary>
        /// Get the list of all deployments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Queryable]
        public IQueryable<DeployResult> GetDeployResults()
        {
            using (_tracer.Step("DeploymentService.GetDeployResults"))
            {
                return GetResults(Request).AsQueryable();
            }
        }

        /// <summary>
        /// Get the list of log entries for a deployment
        /// </summary>
        /// <param name="id">id of the deployment</param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<LogEntry> GetLogEntry(string id)
        {
            using (_tracer.Step("DeploymentService.GetLogEntry"))
            {
                try
                {
                    IEnumerable<LogEntry> deployments = _deploymentManager.GetLogEntries(id).ToList();
                    foreach (var entry in deployments)
                    {
                        if (entry.HasDetails)
                        {
                            entry.DetailsUrl = UriHelper.MakeRelative(Request.RequestUri, entry.Id);
                        }
                    }

                    return deployments;
                }
                catch (FileNotFoundException ex)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex));
                }
            }
        }

        /// <summary>
        /// Get the list of log entry details for a log entry
        /// </summary>
        /// <param name="id">id of the deployment</param>
        /// <param name="logId">id of the log entry</param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<LogEntry> GetLogEntryDetails(string id, string logId)
        {
            using (_tracer.Step("DeploymentService.GetLogEntryDetails"))
            {
                try
                {
                    return _deploymentManager.GetLogEntryDetails(id, logId).ToList();
                }
                catch (FileNotFoundException ex)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, ex));
                }
            }
        }

        /// <summary>
        /// Get a deployment
        /// </summary>
        /// <param name="id">id of the deployment</param>
        /// <returns></returns>
        [HttpGet]
        public DeployResult GetResult(string id)
        {
            using (_tracer.Step("DeploymentService.GetResult"))
            {
                DeployResult result = _deploymentManager.GetResult(id);

                if (result == null)
                {
                    var response = Request.CreateErrorResponse(HttpStatusCode.NotFound, String.Format(CultureInfo.CurrentCulture,
                                                                       Resources.Error_DeploymentNotFound,
                                                                       id));
                    throw new HttpResponseException(response);
                }

                result.Url = Request.RequestUri;
                result.LogUrl = UriHelper.MakeRelative(Request.RequestUri, "log");

                return result;
            }
        }

        private IEnumerable<DeployResult> GetResults(HttpRequestMessage request)
        {
            foreach (var result in _deploymentManager.GetResults())
            {
                result.Url = UriHelper.MakeRelative(request.RequestUri, result.Id);
                result.LogUrl = UriHelper.MakeRelative(request.RequestUri, result.Id + "/log");
                yield return result;
            }
        }

        private JObject GetJsonContent()
        {
            try
            {
                return Request.Content.ReadAsAsync<JObject>().Result;
            }
            catch
            {
                // We're going to return null here since we don't want to force a breaking change
                // on the client side. If the incoming request isn't application/json, we want this 
                // to return null.
                return null;
            }
        }
    }
}
