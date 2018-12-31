using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MicroFocus.Adm.Octane.Api.Core.Connector.Authentication
{
    public class LwssoAuthenticationStrategy : AuthenticationStrategy
    {
        public static string AUTHENTICATION_URL = "/authentication/sign_in";
        public static string DISCONNECT_URL = "/authentication/sign_out";

        private static string LWSSO_COOKIE_NAME = "LWSSO_COOKIE_KEY";
        private static string OCTANE_USER_COOKIE_NAME = "OCTANE_USER";

        private UserPassConnectionInfo credentials;
        private string lwSsoCookie;
        private string octaneUserCookie;
        private String host;

        public LwssoAuthenticationStrategy(ConnectionInfo credentials)
        {
            this.credentials = credentials as UserPassConnectionInfo;
        }


        public async Task<bool> ConnectAsync(String host)
        {
            this.host = host;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(this.host + AUTHENTICATION_URL);

            httpWebRequest.Method = RestConnector.METHOD_POST;
            httpWebRequest.ContentType = RestConnector.CONTENT_TYPE_JSON;
            httpWebRequest.CookieContainer = new CookieContainer();

            Stream stream = await httpWebRequest.GetRequestStreamAsync().ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            using (var streamWriter = new StreamWriter(stream))
            {
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                String json = jsSerializer.Serialize(credentials);
                streamWriter.Write(json);
            }

            using (var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync().ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext))
            {
                SaveCookies(httpResponse);
            }

            return IsConnected();
        }

        public bool IsConnected()
        {
            return GetLwSsoToken() != null;
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
            lwSsoCookie = null;
            octaneUserCookie = null;

            return true;
        }

        public void OnResponse(HttpWebResponse response)
        {
            SaveCookies(response);
        }

        private string GetLwSsoToken()
        {
            return lwSsoCookie;
        }

        /// <summary>
        ///DON'T USE DIRECTLY. Only Testing API
        /// </summary>
        internal void SetLwSsoToken(string token)
        {
            lwSsoCookie = token;
        }


        private void SaveCookies(HttpWebResponse httpResponse)
        {
            lwSsoCookie = ExtractValueFromCookie(httpResponse, LWSSO_COOKIE_NAME, lwSsoCookie);
            octaneUserCookie = ExtractValueFromCookie(httpResponse, OCTANE_USER_COOKIE_NAME, octaneUserCookie);

            string[] setCookies = httpResponse.Headers.GetValues("Set-Cookie");
            lwSsoCookie = ExtractValueFromSetCookie(setCookies, LWSSO_COOKIE_NAME, lwSsoCookie);
            octaneUserCookie = ExtractValueFromSetCookie(setCookies, OCTANE_USER_COOKIE_NAME, octaneUserCookie);
        }

        private static string ExtractValueFromCookie(HttpWebResponse httpResponse, string key, string defaultValue)
        {
            if (httpResponse.Cookies[key] != null)
            {
                return httpResponse.Cookies[key].Value;
            }
            return defaultValue;
        }

        private static string ExtractValueFromSetCookie(string[] setCookieValues, string key, string defaultValue)
        {
            if (setCookieValues != null)
            {
                foreach (string setValue in setCookieValues)
                {
                    if (setValue.StartsWith(key))
                    {
                        //OCTANE_USER=workspace.admin;Version=1;Domain=212.83.136.98;Path=/;Max-Age=86400;Secure;HttpOnly
                        Regex regex = new Regex(key + "=(.*?);");
                        Match match = regex.Match(setValue);
                        if (match.Success)
                        {
                            return match.Groups[1].Value;
                        }
                    }
                }
            }

            return defaultValue;
        }

        private string GetCookieValue(HttpWebResponse httpResponse, string cookieName)
        {
            Cookie cookie = httpResponse.Cookies[cookieName];
            if (cookie == null)
            {
                return string.Empty;
            }

            return cookie.Value;
        }

        public void PrepareRequest(HttpWebRequest request)
        {
            //add cookies
            String cookieDomain = request.Address.Host;
            String cookiePath = "/";

            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(new Cookie(LWSSO_COOKIE_NAME, lwSsoCookie, cookiePath, cookieDomain));
            request.CookieContainer.Add(new Cookie(OCTANE_USER_COOKIE_NAME, octaneUserCookie, cookiePath, cookieDomain));
        }

        public async Task<String> GetWorkspaceUser()
        {
            return credentials.user;
        }
    }
}
