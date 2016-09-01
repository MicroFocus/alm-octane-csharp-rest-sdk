using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Hpe.Nga.Api.Core.Connector.Exceptions;

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


        public static string AUTHENTICATION_URL = "/authentication/sign_in";
        public static string DISCONNECT_URL = "/authentication/sign_out";
        public static string SHARED_SPACES_URL = "/api/shared_spaces";

        private static string METHOD_POST = "POST";
        private static string METHOD_GET = "GET";
        private static string METHOD_PUT = "PUT";
        private static string METHOD_DELETE = "DELETE";

        private string host;
        private string loginName;

        #region Singelton


        private RestConnector() { }

        private static RestConnector instance = new RestConnector();
        private String lwssoToken = null;
        private String csrfToken = Guid.NewGuid().ToString();

        public static RestConnector GetInstance()
        {
            return instance;
        }

        #endregion

        public String ConnectedUser
        {
            get
            {
                return loginName;
            }
        }

        public String Host
        {
            get
            {
                return host;
            }
        }

        public bool Connect(string host, string loginName, string password)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            if (loginName == null)
            {
                throw new ArgumentNullException("loginName");
            }


            this.host = host.TrimEnd('/');
            this.loginName = loginName;


            var httpWebRequest = (HttpWebRequest)WebRequest.Create(this.host + AUTHENTICATION_URL);

            httpWebRequest.Method = METHOD_POST;
            httpWebRequest.ContentType = CONTENT_TYPE_JSON;
            httpWebRequest.CookieContainer = new CookieContainer();


            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                ConnectionInfo auth = new ConnectionInfo(loginName, password);
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                String json = jsSerializer.Serialize(auth);
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
            ResponseWrapper wrapper = ExecutePost(DISCONNECT_URL, null);
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

            //add csrf token
            Cookie csrfCookie = new Cookie(CSRF_COOKIE_NAME, csrfToken, cookiePath, cookieDomain);
            request.CookieContainer.Add(csrfCookie);
            request.Headers.Add(CSRF_HEADER_NAME, csrfToken);

            //add internal API token
            request.Headers.Add("HPECLIENTTYPE", "HPE_MQM_UI");



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

                default:
                    break;
            }


            return request;
        }

        public ResponseWrapper ExecuteGet(string restRelativeUri)
        {
            return Send(restRelativeUri, RequestType.Get, null);
        }

        public ResponseWrapper ExecutePost(string restRelativeUri, string data)
        {
            return Send(restRelativeUri, RequestType.Post, data);
        }

        public ResponseWrapper ExecutePut(string restRelativeUri, string data)
        {
            return Send(restRelativeUri, RequestType.Update, data);
        }

        public ResponseWrapper ExecuteDelete(string restRelativeUri)
        {
            return Send(restRelativeUri, RequestType.Delete, null);
        }

        public ResponseWrapper Send(string restRelativeUri, RequestType requestType, string data)
        {
            if (!IsConnected())
            {
                throw new NotConnectedException();
            }

            HttpWebRequest request = CreateRequest(restRelativeUri, requestType);
            ResponseWrapper responseWrapper = new ResponseWrapper();

            try
            {
                if ((requestType == RequestType.Post || requestType == RequestType.Update) && !String.IsNullOrEmpty(data))
                {
                    byte[] byteData = Encoding.UTF8.GetBytes(data);
                    request.ContentLength = byteData.Length;
                    using (Stream postStream = request.GetRequestStream())
                    {
                        postStream.Write(byteData, 0, byteData.Length);
                    }
                }


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
