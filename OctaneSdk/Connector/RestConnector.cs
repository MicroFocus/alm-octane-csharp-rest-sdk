/*
 * Copyright 2016-2026 Open Text.
 *
 * The only warranties for products and services of Open Text and
 * its affiliates and licensors (“Open Text”) are as may be set forth
 * in the express warranty statements accompanying such products and services.
 * Nothing herein should be construed as constituting an additional warranty.
 * Open Text shall not be liable for technical or editorial errors or
 * omissions contained herein. The information contained herein is subject
 * to change without notice.
 *
 * Except as specifically indicated otherwise, this document contains
 * confidential information and a valid license is required for possession,
 * use or copying. If this work is provided to the U.S. Government,
 * consistent with FAR 12.211 and 12.212, Commercial Computer Software,
 * Computer Software Documentation, and Technical Data for Commercial Items are
 * licensed to the U.S. Government under vendor's standard commercial license.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *   http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using MicroFocus.Adm.Octane.Api.Core.Connector.Authentication;
using MicroFocus.Adm.Octane.Api.Core.Connector.Exceptions;
using MicroFocus.Adm.Octane.Api.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MicroFocus.Adm.Octane.Api.Core.Connector
{
    /// <summary>
    /// Low-level class for communication with NGA server.
    /// The login should be executed first by calling to <see cref="Connect"/> method.
    /// For OO wrapper class <see cref="EntityService" class/>
    /// </summary>
    public class RestConnector
    {

        private static string CONTENT_TYPE_STREAM = "application/octet-stream";
        private static string CONTENT_TYPE_MULTIPART = "multipart/form-data; boundary=";

        public static string CONTENT_TYPE_JSON = "application/json";
        public static string SHARED_SPACES_URL = "/api/shared_spaces";
        public static string METHOD_POST = "POST";
        public static string METHOD_GET = "GET";
        public static string METHOD_PUT = "PUT";
        public static string METHOD_DELETE = "DELETE";
        private AuthenticationStrategy authenticationStrategy;

        private Dictionary<HttpWebRequest, OngoingRequest> ongoingRequests = new Dictionary<HttpWebRequest, OngoingRequest>();

        public string Host { get; private set; }


        public static bool AwaitContinueOnCapturedContext { get; set; } = true;

        public bool Connect(string host, ConnectionInfo connectionInfo)
        {
            return ConnectAsync(host, connectionInfo).Result;
        }

        public bool Connect(string host, AuthenticationStrategy authenticationStrategy)
        {
            return ConnectAsync(host, authenticationStrategy).Result;
        }

        private async Task<bool> Reconnect()
        {
            try
            {
                return await ConnectAsync(Host, authenticationStrategy);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ConnectAsync(string host, ConnectionInfo connectionInfo)
        {
            return await ConnectAsync(host, new LwssoAuthenticationStrategy(connectionInfo)).ConfigureAwait(AwaitContinueOnCapturedContext);
        }

        public async Task<bool> ConnectAsync(string host, AuthenticationStrategy authenticationStrategy)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            if (authenticationStrategy == null)
            {
                throw new ArgumentNullException("authenticationStrategy");
            }

            this.authenticationStrategy = authenticationStrategy;
            this.Host = host.TrimEnd('/');

            return await authenticationStrategy.ConnectAsync(this.Host).ConfigureAwait(AwaitContinueOnCapturedContext);
        }

        public async Task<bool> DisconnectAsync()
        {
            CloseAllOngoing();
            return await authenticationStrategy.DisconnectAsync().ConfigureAwait(AwaitContinueOnCapturedContext);
        }

        public void Disconnect()
        {
            bool disconnected = DisconnectAsync().Result;
        }

        public bool IsConnected()
        {
            return authenticationStrategy != null && authenticationStrategy.IsConnected();
        }

        private HttpWebRequest CreateRequest(string restRelativeUri, RequestType requestType, RequestConfiguration additionalRequestConfiguration)
        {
            string url = Host + restRelativeUri;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (NetworkSettings.CustomProxy != null)
            {
                request.Proxy = NetworkSettings.CustomProxy;
            }

            //Add authentication cookies/headers
            authenticationStrategy.PrepareRequest(request);


            //add internal API token
            request.Headers.Add("HPECLIENTTYPE", "HPE_CI_CLIENT");


            //set content type/accept/method
            switch (requestType)
            {
                case RequestType.Get:
                    request.Accept = CONTENT_TYPE_JSON;
                    request.Method = METHOD_GET;
                    break;

                case RequestType.GetOctet:
                    request.Accept = CONTENT_TYPE_STREAM;
                    request.Method = METHOD_GET;
                    break;

                case RequestType.Post:
                    request.ContentType = CONTENT_TYPE_JSON;
                    request.Accept = CONTENT_TYPE_JSON;
                    request.Method = METHOD_POST;
                    break;

                case RequestType.PostOctet:
                    request.ContentType = CONTENT_TYPE_STREAM;
                    request.Method = METHOD_POST;
                    break;

                case RequestType.Delete:
                    request.Accept = CONTENT_TYPE_JSON;
                    request.Method = METHOD_DELETE;
                    break;

                case RequestType.Update:
                    request.ContentType = CONTENT_TYPE_JSON;
                    request.Accept = CONTENT_TYPE_JSON;
                    request.Method = METHOD_PUT;
                    break;

                case RequestType.MultiPart:
                    request.ContentType = CONTENT_TYPE_MULTIPART;
                    request.Accept = CONTENT_TYPE_JSON;
                    request.Method = METHOD_POST;
                    break;

                default:
                    break;
            }

            if (additionalRequestConfiguration != null)
            {
                if (additionalRequestConfiguration.Timeout.HasValue)
                {
                    request.Timeout = additionalRequestConfiguration.Timeout.Value;
                }
                if (additionalRequestConfiguration.Headers != null)
                {
                    foreach (KeyValuePair<string, string> header2value in additionalRequestConfiguration.Headers)
                    {
                        switch (header2value.Key.ToLower())
                        {
                            case "contenttype":
                                request.ContentType = header2value.Value;
                                break;
                            case "accept":
                                request.Accept = header2value.Value;
                                break;
                            default:
                                request.Headers.Add(header2value.Key, header2value.Value);
                                break;
                        }
                    }
                }
                if (additionalRequestConfiguration.GZipCompression)
                {
                    request.Headers.Add("Content-Encoding", "gzip");
                }
            }

            return request;
        }

        public ResponseWrapper ExecuteGet(string restRelativeUri, string queryParams, RequestConfiguration additionalRequestConfiguration = null)
        {
            return ExecuteGetAsync(restRelativeUri, queryParams, additionalRequestConfiguration).Result;
        }

        public Task<ResponseWrapper> ExecuteGetAsync(string restRelativeUri, string queryParams, RequestConfiguration additionalRequestConfiguration = null)
        {
            return SendAsync(restRelativeUri, queryParams, RequestType.Get, null, true, additionalRequestConfiguration);
        }

        public ResponseWrapper ExecutePost(string restRelativeUri, string queryParams, string data, RequestConfiguration additionalRequestConfiguration = null)
        {
            return ExecutePostAsync(restRelativeUri, queryParams, data, additionalRequestConfiguration).Result;
        }

        public Task<ResponseWrapper> ExecutePostAsync(string restRelativeUri, string queryParams, string data, RequestConfiguration additionalRequestConfiguration = null)
        {
            return SendAsync(restRelativeUri, queryParams, RequestType.Post, data, true, additionalRequestConfiguration);
        }

        public ResponseWrapper ExecutePut(string restRelativeUri, string queryParams, string data, RequestConfiguration additionalRequestConfiguration = null)
        {
            return ExecutePutAsync(restRelativeUri, queryParams, data, additionalRequestConfiguration).Result;
        }

        public Task<ResponseWrapper> ExecutePutAsync(string restRelativeUri, string queryParams, string data, RequestConfiguration additionalRequestConfiguration = null)
        {
            return SendAsync(restRelativeUri, queryParams, RequestType.Update, data, true, additionalRequestConfiguration);
        }

        public ResponseWrapper ExecuteDelete(string restRelativeUri, RequestConfiguration additionalRequestConfiguration = null)
        {
            return ExecuteDeleteAsync(restRelativeUri, additionalRequestConfiguration).Result;
        }

        public Task<ResponseWrapper> ExecuteDeleteAsync(string restRelativeUri, RequestConfiguration additionalRequestConfiguration = null)
        {
            return SendAsync(restRelativeUri, null, RequestType.Delete, null, true, additionalRequestConfiguration);
        }

        private async Task<ResponseWrapper> DoSendAsync(HttpWebRequest request)
        {
            ResponseWrapper responseWrapper = new ResponseWrapper();

            try
            {
                AddToOngoing(request);
                var response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(AwaitContinueOnCapturedContext);
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    responseWrapper.Data = streamReader.ReadToEnd();
                }

                responseWrapper.StatusCode = response.StatusCode;
                authenticationStrategy.OnResponse(response);

            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                if (response == null)
                {
                    throw new ServerUnavailableException("Server is unavailable", ex);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new InvalidCredentialException("Credentials are invalid");
                    }
                    if (response.StatusCode == HttpStatusCode.RequestTimeout)
                    {
                        throw new WebException(ex.Message, WebExceptionStatus.Timeout);
                    }

                    string body = string.Empty;
                    try
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            body = streamReader.ReadToEnd();
                        }
                    }
                    catch
                    {
                        // If anything goes wrong in the reading of the server response we still want to throw
                        // an exception with the original exception as inner.
                        throw ex;
                    }

                    try
                    {
                        JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                        if (body.Contains("total_count"))
                        {
                            RestExceptionInfos exceptionInfos = jsSerializer.Deserialize<RestExceptionInfos>(body);
                            throw new MqmRestException(exceptionInfos.errors[0], response.StatusCode, ex);
                        }
                        else
                        {
                            RestExceptionInfo exceptionInfo = jsSerializer.Deserialize<RestExceptionInfo>(body);
                            throw new MqmRestException(exceptionInfo, response.StatusCode, ex);
                        }
                    }
                    catch (Exception e)
                    {
                        if (e is MqmRestException)
                        {
                            throw e;
                        }
                        else
                        {
                            throw new GeneralHttpException(body, response.StatusCode);
                        }
                    }
                }
            }
            finally
            {
                RemoveFromOngoing(request);
            }

            return responseWrapper;
        }

        public ResponseWrapper Send(string restRelativeUri, string queryParams, RequestType requestType, string data, RequestConfiguration additionalData = null)
        {
            return SendAsync(restRelativeUri, queryParams, requestType, data, true, additionalData).Result;
        }

        public async Task<ResponseWrapper> SendAsync(string restRelativeUri, string queryParams, RequestType requestType, string data, bool allowReconnect = true, RequestConfiguration additionalRequestConfiguration = null)
        {
            if (!IsConnected())
            {
                throw new NotConnectedException();
            }

            restRelativeUri = string.IsNullOrWhiteSpace(queryParams) ? restRelativeUri : restRelativeUri + (restRelativeUri.Contains("?") ? "&" : "?") + queryParams;
            HttpWebRequest request = CreateRequest(restRelativeUri, requestType, additionalRequestConfiguration);

            if ((requestType == RequestType.Post || requestType == RequestType.Update) && !string.IsNullOrEmpty(data))
            {
                byte[] byteData = Encoding.UTF8.GetBytes(data);
                bool gzip = additionalRequestConfiguration != null && additionalRequestConfiguration.GZipCompression;
                if (gzip)
                {
                    using (Stream postStream = await request.GetRequestStreamAsync().ConfigureAwait(AwaitContinueOnCapturedContext))
                    {
                        using (var zipStream = new GZipStream(postStream, CompressionMode.Compress))
                        {
                            zipStream.Write(byteData, 0, byteData.Length);
                        }
                    }
                }
                else
                {
                    request.ContentLength = byteData.Length;
                    using (Stream postStream = await request.GetRequestStreamAsync().ConfigureAwait(AwaitContinueOnCapturedContext))
                    {
                        postStream.Write(byteData, 0, byteData.Length);
                    }
                }
            }

            InvalidCredentialException originalException = null;
            try
            {
                if (requestType == RequestType.GetOctet && additionalRequestConfiguration != null)
                {
                    return await DownloadAttachmentInternalAsync(request, additionalRequestConfiguration.AttachmentDownloadPath).ConfigureAwait(AwaitContinueOnCapturedContext);
                }
                else
                {
                    return await DoSendAsync(request).ConfigureAwait(AwaitContinueOnCapturedContext);
                }
            }
            //catch - intended to handle the case LWSSO is expired, we will try reconnect and resend original request
            catch (InvalidCredentialException e)
            {
                // await can't exist in a catch because the CLR would lose the ambient exception.
                // We don’t need the ambient exception (i.e. we don't "throw;", 
                // so we need to trick it out by putting retry logic after the catch
                originalException = e;
            }

            //RECONNECT SECTION
            try
            {
                bool reconnected = allowReconnect && await Reconnect();
                if (!reconnected)
                {
                    throw originalException;
                }
                // we reconnected, 
                // but await can't exist in a catch because the CLR would lose the ambient exception.
                // We don’t need the ambient exception (i.e. we don't "throw;", 
                // so we need to trick it out by putting retry logic after the catch
            }
            catch
            {
                //if reconnect throwed any exception - we rethrow original exception
                throw originalException;
            }

            //resend after reconnect
            return await SendAsync(restRelativeUri, queryParams, requestType, data, false, additionalRequestConfiguration)
                   .ConfigureAwait(AwaitContinueOnCapturedContext);
        }

        public ResponseWrapper SendMultiPart(string restRelativeUrl, byte[] binaryContent, string binaryContentType, string fileName, string entityData)
        {
            return SendMultiPartAsync(restRelativeUrl, binaryContent, binaryContentType, fileName, entityData).Result;
        }

        public async Task<ResponseWrapper> SendMultiPartAsync(string restRelativeUrl, byte[] binaryContent, string binaryContentType, string fileName, string entityData)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = CreateRequest(restRelativeUrl, RequestType.MultiPart, null);
            wr.ContentType += boundary;

            Stream rs = await wr.GetRequestStreamAsync().ConfigureAwait(AwaitContinueOnCapturedContext);

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"blob\"\r\nContent-Type: application/json\r\n\r\n{1}";

            rs.Write(boundarybytes, 0, boundarybytes.Length);
            string formItem = string.Format(formdataTemplate, "entity", entityData);
            byte[] formitembytes = Encoding.UTF8.GetBytes(formItem);
            rs.Write(formitembytes, 0, formitembytes.Length);

            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"content\"; filename=\"{0}\"\r\nContent-Type: {1}\r\n\r\n";
            string header = string.Format(headerTemplate, fileName, binaryContentType);
            byte[] headerbytes = Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            using (MemoryStream ms = new MemoryStream(binaryContent))
            {
                while ((bytesRead = ms.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
            }

            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            return await DoSendAsync(wr).ConfigureAwait(AwaitContinueOnCapturedContext);
        }

        /// <summary>
        /// Download the attachment at the url and store it locally at the given location
        /// </summary>
        public async Task DownloadAttachmentAsync(string relativeUrl, string destinationPath)
        {
            var additionalRequestConfiguration = new RequestConfiguration();
            additionalRequestConfiguration.AttachmentDownloadPath = destinationPath;

            await SendAsync(relativeUrl, null, RequestType.GetOctet, null, true, additionalRequestConfiguration);
        }

        private async Task<ResponseWrapper> DownloadAttachmentInternalAsync(HttpWebRequest request, string destinationPath)
        {
            ResponseWrapper responseWrapper = new ResponseWrapper();

            var response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(AwaitContinueOnCapturedContext);

            byte[] buffer = new byte[1024];

            using (var writeStream = File.Open(destinationPath, FileMode.OpenOrCreate))
            using (Stream input = response.GetResponseStream())
            {
                // copy the response stream contents to a local file
                int size = input.Read(buffer, 0, buffer.Length);
                while (size > 0)
                {
                    writeStream.Write(buffer, 0, size);
                    size = input.Read(buffer, 0, buffer.Length);
                }
                writeStream.Flush();
            }

            responseWrapper.StatusCode = response.StatusCode;

            return responseWrapper;
        }



        private void AddToOngoing(HttpWebRequest request)
        {
            lock (this)
            {
                ongoingRequests.Add(request, OngoingRequest.Create(request));
            }
        }

        private void RemoveFromOngoing(HttpWebRequest request)
        {
            lock (this)
            {
                ongoingRequests.Remove(request);
            }
        }

        private void CloseAllOngoing()
        {
            lock (this)
            {
                foreach (KeyValuePair<HttpWebRequest, OngoingRequest> entry in ongoingRequests)
                {
                    entry.Value.Request.Abort();
                }
            }
        }

        public IList<OngoingRequest> GetOngoingRequests()
        {
            lock (this)
            {
                return new List<OngoingRequest>(ongoingRequests.Values);
            }
        }
    }
}
