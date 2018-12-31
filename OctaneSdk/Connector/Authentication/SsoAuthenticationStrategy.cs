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

        private string cookieName;
        private string cookieValue;
        private string DEFAULT_OCTANE_SESSION_COOKIE_NAME = "LWSSO_COOKIE_KEY";
        private string DISCONNECT_URL = "/authentication/sign_out";
        private string AUTHENTICATION_URL = "/authentication/grant_tool_token";
        private string octaneUserValue;
        private string host;
        private APIKeyConnectionInfo connectionInfo;
        private long pollingTimeoutSeconds = 60 * 2; // 2 minutes

        private void ClearSessionFields()
        {
            cookieName = DEFAULT_OCTANE_SESSION_COOKIE_NAME;
            cookieValue = "";
            octaneUserValue = "";
        }

        
        public async Task<bool> ConnectAsync(string host)
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
                
            using (var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync().ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext))
            {
                using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    connectionInfo = (APIKeyConnectionInfo)js.Deserialize(objText, typeof(APIKeyConnectionInfo));
                }
            }

            long pollingTimeoutTimestamp = 0;
            APIKeyConnectionInfo accessTokenConnectionInfo = new APIKeyConnectionInfo();

            while (pollingTimeoutTimestamp > pollingTimeoutSeconds)
            {
                    
                try
                {
                    var pollRequest = (HttpWebRequest)WebRequest.Create(this.host + AUTHENTICATION_URL);

                    pollRequest.Method = RestConnector.METHOD_POST;
                    pollRequest.ContentType = RestConnector.CONTENT_TYPE_JSON;
                    pollRequest.CookieContainer = new CookieContainer();

                    Stream stream = await pollRequest.GetRequestStreamAsync().ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
                    using (var streamWriter = new StreamWriter(stream))
                    {
                        JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                        String json = jsSerializer.Serialize(connectionInfo.identifier);
                        streamWriter.Write(json);
                    }

                    using (var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync().ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext))
                    {
                        using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            JavaScriptSerializer js = new JavaScriptSerializer();
                            var objText = reader.ReadToEnd();
                            accessTokenConnectionInfo = (APIKeyConnectionInfo)js.Deserialize(objText, typeof(APIKeyConnectionInfo));
                        }
                    }


                }
                catch (Exception ex)
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
            return null;
        }
    }
}
