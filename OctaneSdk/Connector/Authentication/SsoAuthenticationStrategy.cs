/*!
* (c) 2016-2018 EntIT Software LLC, a Micro Focus company
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using MicroFocus.Adm.Octane.Api.Core.Connector.Credentials;
using MicroFocus.Adm.Octane.Api.Core.Entities;
using MicroFocus.Adm.Octane.Api.Core.Services.Core;
using MicroFocus.Adm.Octane.Api.Core.Services.Version;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MicroFocus.Adm.Octane.Api.Core.Connector.Authentication
{
    public class SsoAuthenticationStrategy : AuthenticationStrategy
    {
        private Object authenticationLock = new Object();
        private string cookieName;
        private string cookieValue;
        private string DEFAULT_OCTANE_SESSION_COOKIE_NAME = "LWSSO_COOKIE_KEY";
        private string DISCONNECT_URL = "/authentication/sign_out";
        private string AUTHENTICATION_URL = "/authentication/grant_tool_token";
        private string octaneUserValue;
        private string host;
        private SsoConnectionInfo connectionInfo;
        private long pollingTimeoutSeconds = 60 * 2; // 2 minutes
        private ConnectionListener connectionListener;
        private JavaScriptSerializer jSerialiser;

        private readonly Mutex mutex;

        public SsoAuthenticationStrategy()
        {
            jSerialiser = new JavaScriptSerializer();
            jSerialiser.RegisterConverters(new JavaScriptConverter[] { new EntityJsonConverter(), new EntityIdJsonConverter() });

            mutex = new Mutex();
        }

        private void ClearSessionFields()
        {
            cookieName = DEFAULT_OCTANE_SESSION_COOKIE_NAME;
            cookieValue = "";
            octaneUserValue = "";
        }

        
        public async Task<bool> ConnectAsync(string host)
        {

            return await System.Threading.Tasks.Task.Run(() =>
                { 
                    mutex.WaitOne();
                    try
                    {
                        ClearSessionFields();

                        this.host = host;
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(this.host + AUTHENTICATION_URL);

                        httpWebRequest.Method = RestConnector.METHOD_GET;
                        httpWebRequest.ContentType = RestConnector.CONTENT_TYPE_JSON;
                        httpWebRequest.CookieContainer = new CookieContainer();

                        using (var httpResponse = httpWebRequest.GetResponse())
                        {
                            using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var objText = reader.ReadToEnd();
                                connectionInfo = (SsoConnectionInfo)jSerialiser.Deserialize(objText, typeof(SsoConnectionInfo));
                            }
                        }

                        if (connectionListener != null)
                        {
                            connectionListener.OpenBrowser(connectionInfo.authentication_url);
                        }

                        long pollingTimeoutTimestamp = pollingTimeoutSeconds;
                        SsoConnectionInfo accessTokenConnectionInfo = new SsoConnectionInfo();

                        while (pollingTimeoutTimestamp >= 0)
                        {

                            try
                            {
                                // check whether the user closed the browser dialg
                                if (connectionListener != null)
                                {
                                    if (!connectionListener.IsOpen())
                                    {
                                        break;
                                    }
                                }
                                var pollRequest = (HttpWebRequest)WebRequest.Create(this.host + AUTHENTICATION_URL);

                                pollRequest.Method = RestConnector.METHOD_POST;
                                pollRequest.ContentType = RestConnector.CONTENT_TYPE_JSON;
                                pollRequest.CookieContainer = new CookieContainer();

                                Stream stream = pollRequest.GetRequestStream();
                                using (var streamWriter = new StreamWriter(stream))
                                {
                                    String json = jSerialiser.Serialize(connectionInfo);
                                    streamWriter.Write(json);
                                }

                                using (var httpResponse = pollRequest.GetResponse())                        
                                using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                                {
                                    var objText = reader.ReadToEnd();
                                    accessTokenConnectionInfo = (SsoConnectionInfo)jSerialiser.Deserialize(objText, typeof(SsoConnectionInfo));
                                }
                        
                        
                            }
                            catch (Exception)
                            {
                                Thread.Sleep(1000); // Do not DOS the server, not cool  
                                pollingTimeoutTimestamp--;
                                // update connection listener on timeout progress
                                if (connectionListener != null)
                                {
                                    connectionListener.UpdateTimeout((int) pollingTimeoutTimestamp);
                                }
                                continue;
                            }

                            cookieValue = accessTokenConnectionInfo.access_token;
                            cookieName = accessTokenConnectionInfo.cookie_name;

                            if (connectionListener != null)
                            {
                                connectionListener.CloseBrowser();
                            }
                            return true;
                        }

                        // check whether the user forgot about the browser dialg
                        if (connectionListener != null)
                        {
                            if (connectionListener.IsOpen())
                            {
                                connectionListener.CloseBrowser();
                            }
                        }

                        return false;
                    }
                finally
                {
                    mutex.ReleaseMutex();
                }
                });
        }

    
        public async Task<bool> DisconnectAsync()
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(this.host + DISCONNECT_URL);
                PrepareRequest(httpWebRequest);
                httpWebRequest.Method = RestConnector.METHOD_POST;
                using (var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync().ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext))
                {
                }

            }
            catch (Exception)
            {
                // Do nothing
            }

            // Reset cookies container to erase any existing cookies of the previous session.
            cookieValue = null;
            cookieName = null;

            return true;
        }

        public bool IsConnected()
        {
            return  cookieValue != null && !cookieValue.Equals("") ;
        }

        public void OnResponse(HttpWebResponse response)
        {
            // don't need to implement this, access tokens do not get refreshed only on connect
        }
        
        public void PrepareRequest(HttpWebRequest request)
        {
            //add cookies
            String cookieDomain = request.Address.Host;
            String cookiePath = "/";

            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(new Cookie(cookieName, cookieValue, cookiePath, cookieDomain));
            
        }

        public async Task<String> GetWorkspaceUser()
        {
            if(octaneUserValue != null && !octaneUserValue.Equals(""))
            {
                return octaneUserValue;
            }

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(host + "/api/current_user/");

                httpWebRequest.Method = RestConnector.METHOD_GET;
                httpWebRequest.ContentType = RestConnector.CONTENT_TYPE_JSON;

                PrepareRequest(httpWebRequest);

                httpWebRequest.Headers.Add("HPECLIENTTYPE", "HPE_CI_CLIENT");

                ResponseWrapper responseWrapper = new ResponseWrapper();

                using (var httpResponse = await httpWebRequest.GetResponseAsync().ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext))
                {
                    using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        responseWrapper.Data = reader.ReadToEnd();
                    }
                }
                WorkspaceUser result = jSerialiser.Deserialize<WorkspaceUser>(responseWrapper.Data);
                octaneUserValue = result.Name;
                return result.Name;
            }
            catch (Exception)
            {
                return null; 
            }
        }

        public void SetConnectionListener(ConnectionListener connectionListener)
        {
            this.connectionListener = connectionListener;
        }

        public async Task<bool> TestConnection(string host)
        {
            // determine if sso is implemented in octane server
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(host + "/admin/server/version");

            httpWebRequest.Method = RestConnector.METHOD_GET;
            httpWebRequest.ContentType = RestConnector.CONTENT_TYPE_JSON;

            httpWebRequest.Headers.Add("HPECLIENTTYPE", "HPE_CI_CLIENT");

            ResponseWrapper responseWrapper = new ResponseWrapper();

            using (var httpResponse = await httpWebRequest.GetResponseAsync().ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext))
            {
                using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseWrapper.Data = reader.ReadToEnd();
                }
            }

            OctaneVersion octaneVersion = new OctaneVersion(jSerialiser.Deserialize<OctaneVersionMetadata>(responseWrapper.Data).display_version);

            if(octaneVersion.CompareTo(OctaneVersion.INTER_P2) < 0)
            {
                throw new Exception("Login with browser is only supported starting from Octane server version: " + OctaneVersion.INTER_P2);
            }
            else
            {
                return true;
            }
            
        }
    }
}
