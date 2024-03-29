﻿/*
 * Copyright 2016-2024 Open Text.
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

using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MicroFocus.Adm.Octane.Api.Core.Connector.Authentication
{
    public class LwssoAuthenticationStrategy : AuthenticationStrategy
    {
        public static string AUTHENTICATION_URL = "/authentication/sign_in";
        public static string DISCONNECT_URL = "/authentication/sign_out";

        private static readonly string LWSSO_COOKIE_NAME = "LWSSO_COOKIE_KEY";
        private static readonly string OCTANE_USER_COOKIE_NAME = "OCTANE_USER";

        private ConnectionInfo credentials;
        private string lwSsoCookie;
        private string octaneUserCookie;
        private string host;

        public LwssoAuthenticationStrategy(ConnectionInfo credentials)
        {
            this.credentials = credentials;
        }


        public async Task<bool> ConnectAsync(string host)
        {
            this.host = host;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(this.host + AUTHENTICATION_URL);
            if (NetworkSettings.CustomProxy != null)
            {
                httpWebRequest.Proxy = NetworkSettings.CustomProxy;
            }

            httpWebRequest.Method = RestConnector.METHOD_POST;
            httpWebRequest.ContentType = RestConnector.CONTENT_TYPE_JSON;
            httpWebRequest.CookieContainer = new CookieContainer();

            Stream stream = await httpWebRequest.GetRequestStreamAsync().ConfigureAwait(RestConnector.AwaitContinueOnCapturedContext);
            using (var streamWriter = new StreamWriter(stream))
            {
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                string json = jsSerializer.Serialize(credentials);
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
                if (NetworkSettings.CustomProxy != null)
                {
                    httpWebRequest.Proxy = NetworkSettings.CustomProxy;
                }
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
            string cookieDomain = request.Address.Host;
            string cookiePath = "/";

            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(new Cookie(LWSSO_COOKIE_NAME, lwSsoCookie, cookiePath, cookieDomain));
            request.CookieContainer.Add(new Cookie(OCTANE_USER_COOKIE_NAME, octaneUserCookie, cookiePath, cookieDomain));
        }

        public Task<string> GetWorkspaceUser()
        {
            string user = string.Empty;
            if(credentials is UserPassConnectionInfo)
            {
                user = ((UserPassConnectionInfo) credentials).user;
            }
            else
            {
                user = ((APIKeyConnectionInfo)credentials).client_id;
            }
            return Task.FromResult(user);
        }

        public async Task<bool> TestConnection(string host)
        {
            try
            {
                return await ConnectAsync(host);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
