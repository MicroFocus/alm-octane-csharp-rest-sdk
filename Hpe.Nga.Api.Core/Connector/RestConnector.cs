// (c) Copyright 2016 Hewlett Packard Enterprise Development LP

// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.

// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,

// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Hpe.Nga.Api.Core.Connector.Exceptions;
using System.Collections.Specialized;

namespace Hpe.Nga.Api.Core.Connector
{
    /// <summary>
    /// Low-level class for communication with NGA server.
    /// Used as singelton.
    /// The login should be executed first by calling to <see cref="Connect"/> method.
    /// For OO wrapper class <see cref="EntityService" class/>
    /// </summary>
    public class RestConnector
    {
        private static string LWSSO_COOKIE_NAME = "LWSSO_COOKIE_KEY";
        private static string CSRF_COOKIE_NAME = "HPSSO_COOKIE_CSRF";
        private static string CSRF_HEADER_NAME = "HPSSO-HEADER-CSRF";


        private static string CONTENT_TYPE_JSON = "application/json";
        private static string CONTENT_TYPE_STREAM = "application/octet-stream";
        private static string CONTENT_TYPE_MULTIPART = "multipart/form-data; boundary=";


        public static string AUTHENTICATION_URL = "/authentication/sign_in";
        public static string DISCONNECT_URL = "/authentication/sign_out";
        public static string SHARED_SPACES_URL = "/api/shared_spaces";

        private static string METHOD_POST = "POST";
        private static string METHOD_GET = "GET";
        private static string METHOD_PUT = "PUT";
        private static string METHOD_DELETE = "DELETE";

        private string host;

        private String lwssoToken = null;
        private String csrfToken = Guid.NewGuid().ToString();

        public String Host
        {
            get
            {
                return host;
            }
        }

        public bool Connect(string host, ConnectionInfo connectionInfo)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }


            this.host = host.TrimEnd('/');

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(this.host + AUTHENTICATION_URL);

            httpWebRequest.Method = METHOD_POST;
            httpWebRequest.ContentType = CONTENT_TYPE_JSON;
            httpWebRequest.CookieContainer = new CookieContainer();


            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                String json = jsSerializer.Serialize(connectionInfo);
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            if (httpResponse.Cookies[LWSSO_COOKIE_NAME] != null)
            {
                lwssoToken = httpResponse.Cookies[LWSSO_COOKIE_NAME].Value;
            }

            if (httpResponse.Cookies[CSRF_COOKIE_NAME] != null)
            {
                csrfToken = httpResponse.Cookies[CSRF_COOKIE_NAME].Value;
            }


            return lwssoToken != null;
        }

        public void Disconnect()
        {
            ResponseWrapper wrapper = ExecutePost(DISCONNECT_URL, null, null);
            lwssoToken = null;
        }

        public bool IsConnected()
        {
            return lwssoToken != null;
        }

        private HttpWebRequest CreateRequest(string restRelativeUri, RequestType requestType)
        {
            String url = host + restRelativeUri;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            //add cookies
            request.CookieContainer = new CookieContainer();
            String cookieDomain = request.Address.Host;
            String cookiePath = "/";

            //add lwsso token
            Cookie lwssoCookie = new Cookie(LWSSO_COOKIE_NAME, lwssoToken, cookiePath, cookieDomain);
            request.CookieContainer.Add(lwssoCookie);

            //add internal API token
            request.Headers.Add("HPECLIENTTYPE", "HPE_REST_API_TECH_PREVIEW");



            //set content type/accept/method
            switch (requestType)
            {
                case RequestType.Get:
                    request.Accept = CONTENT_TYPE_JSON;
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


            return request;
        }

        public ResponseWrapper ExecuteGet(string restRelativeUri, string queryParams)
        {
            return Send(restRelativeUri, queryParams, RequestType.Get, null);
        }

        public ResponseWrapper ExecutePost(string restRelativeUri, string queryParams, string data)
        {
            return Send(restRelativeUri, queryParams, RequestType.Post, data);
        }

        public ResponseWrapper ExecutePut(string restRelativeUri, string queryParams, string data)
        {
            return Send(restRelativeUri, queryParams, RequestType.Update, data);
        }

        public ResponseWrapper ExecuteDelete(string restRelativeUri)
        {
            return Send(restRelativeUri, null, RequestType.Delete, null);
        }

        private ResponseWrapper DoSend(HttpWebRequest request)
        {
            ResponseWrapper responseWrapper = new ResponseWrapper();

            try
            {

                var response = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    responseWrapper.Data = streamReader.ReadToEnd();
                }

                responseWrapper.StatusCode = response.StatusCode;
                UpdateLwssoTokenFromResponse(response);

            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                if (response == null)
                {
                    throw new ServerUnavailableException();
                }
                else
                {
                    String body = null;
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        body = streamReader.ReadToEnd();
                    }
                    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                    RestExceptionInfo exceptionInfo = jsSerializer.Deserialize<RestExceptionInfo>(body);
                    throw new MqmRestException(exceptionInfo, response.StatusCode);
                }

            }

            return responseWrapper;
        }

        public ResponseWrapper Send(string restRelativeUri, string queryParams, RequestType requestType, string data)
        {
            if (!IsConnected())
            {
                throw new NotConnectedException();
            }

            //Console.WriteLine(requestType + " : " + restRelativeUri);
            restRelativeUri = string.IsNullOrWhiteSpace(queryParams) ? restRelativeUri : restRelativeUri + "?" + queryParams;
            HttpWebRequest request = CreateRequest(restRelativeUri, requestType);
            request.Timeout = 200000;//default 100000



            if ((requestType == RequestType.Post || requestType == RequestType.Update) && !String.IsNullOrEmpty(data))
            {
                byte[] byteData = Encoding.UTF8.GetBytes(data);
                request.ContentLength = byteData.Length;
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }
            }

            return DoSend(request);
            

        }

        public ResponseWrapper SendMultiPart(string restRelativeUrl, Byte[] binaryContent, string binaryContentType, string fileName, string entityData)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = CreateRequest(restRelativeUrl, RequestType.MultiPart);
            wr.ContentType += boundary;
            
            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"blob\"\r\nContent-Type: application/json\r\n\r\n{1}";

            rs.Write(boundarybytes, 0, boundarybytes.Length);
            string formItem = string.Format(formdataTemplate, "entity", entityData);
            byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formItem);
            rs.Write(formitembytes, 0, formitembytes.Length);

            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"content\"; filename=\"{0}\"\r\nContent-Type: {1}\r\n\r\n";
            string header = string.Format(headerTemplate,  fileName, binaryContentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
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

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            return DoSend(wr);
        }

        private void UpdateLwssoTokenFromResponse(HttpWebResponse httpResponse)
        {
            //update security token if it was modified
            String setCookieAll = httpResponse.GetResponseHeader("Set-Cookie");
            String[] setCookies = setCookieAll.Split(';');
            foreach (String setCookie in setCookies)
            {
                if (setCookie.StartsWith(LWSSO_COOKIE_NAME))
                {
                    String[] setCookiesParts = setCookie.Split('=');
                    lwssoToken = setCookiesParts[1];

                }
            }
        }

    }
}
