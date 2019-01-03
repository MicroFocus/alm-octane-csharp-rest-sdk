using MicroFocus.Adm.Octane.Api.Core.Entities;
using MicroFocus.Adm.Octane.Api.Core.Services.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
        private APIKeyConnectionInfo connectionInfo;
        private long pollingTimeoutSeconds = 60 * 2; // 2 minutes
        private ConnectionListener connectionListener;
        private JavaScriptSerializer jSerialiser;

        public SsoAuthenticationStrategy()
        {
            jSerialiser = new JavaScriptSerializer();
            jSerialiser.RegisterConverters(new JavaScriptConverter[] { new EntityJsonConverter(), new EntityIdJsonConverter() });
        }

        private void ClearSessionFields()
        {
            cookieName = DEFAULT_OCTANE_SESSION_COOKIE_NAME;
            cookieValue = "";
            octaneUserValue = "";
        }

        
        public async Task<bool> ConnectAsync(string host)
        {
            lock (authenticationLock)
            {
                // do not authenticate if lwssoValue is not empty
                if (IsConnected())
                {
                    return true;
                }

                ClearSessionFields();

                this.host = host;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(this.host + AUTHENTICATION_URL);

                httpWebRequest.Method = RestConnector.METHOD_GET;
                httpWebRequest.ContentType = RestConnector.CONTENT_TYPE_JSON;
                httpWebRequest.CookieContainer = new CookieContainer();

                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var objText = reader.ReadToEnd();
                        connectionInfo = (APIKeyConnectionInfo)jSerialiser.Deserialize(objText, typeof(APIKeyConnectionInfo));
                    }
                }

                if (connectionListener != null)
                {
                    connectionListener.OpenBrowser(connectionInfo.authentication_url);
                }

                long pollingTimeoutTimestamp = 0;
                APIKeyConnectionInfo accessTokenConnectionInfo = new APIKeyConnectionInfo();

                while (pollingTimeoutTimestamp < pollingTimeoutSeconds)
                {

                    try
                    {
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

                        using (var httpResponse = (HttpWebResponse)pollRequest.GetResponse())                        
                        using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var objText = reader.ReadToEnd();
                            accessTokenConnectionInfo = (APIKeyConnectionInfo)jSerialiser.Deserialize(objText, typeof(APIKeyConnectionInfo));
                        }
                        
                        
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(1000); // Do not DOS the server, not cool    
                        continue;
                    }

                    cookieValue = accessTokenConnectionInfo.access_token;
                    cookieName = accessTokenConnectionInfo.cookie_name;

                    return true;
                }

                return false;
            }
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
            // do nothing yet
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

                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
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
    }
}
