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


using MicroFocus.Adm.Octane.Api.Core.Connector.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MicroFocus.Adm.Octane.Api.Core.Connector
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
		private static string OCTANE_USER_COOKIE_NAME = "OCTANE_USER";

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
		private ConnectionInfo connectionInfo;

		private string lwSsoCookie;
		private string octaneUserCookie;
		private static bool awaitContinueOnCapturedContext = true;

		public String Host
		{
			get
			{
				return host;
			}
		}

		public static bool AwaitContinueOnCapturedContext
		{
			get
			{
				return awaitContinueOnCapturedContext;
			}
			set
			{
				awaitContinueOnCapturedContext = value;
			}
		}

		public bool Connect(string host, ConnectionInfo connectionInfo)
		{
			return ConnectAsync(host, connectionInfo).Result;
		}

		private bool Reconnect()
		{
			try
			{
				return Connect(host, connectionInfo);
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> ConnectAsync(string host, ConnectionInfo connectionInfo)
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
			this.connectionInfo = connectionInfo;

			var httpWebRequest = (HttpWebRequest)WebRequest.Create(this.host + AUTHENTICATION_URL);

			httpWebRequest.Method = METHOD_POST;
			httpWebRequest.ContentType = CONTENT_TYPE_JSON;
			httpWebRequest.CookieContainer = new CookieContainer();

			Stream stream = await httpWebRequest.GetRequestStreamAsync().ConfigureAwait(AwaitContinueOnCapturedContext);
			using (var streamWriter = new StreamWriter(stream))
			{
				JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
				String json = jsSerializer.Serialize(connectionInfo);
				streamWriter.Write(json);
			}

			var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync().ConfigureAwait(AwaitContinueOnCapturedContext);

			SaveCookies(httpResponse);

			return IsConnected();
		}

		private string GetLwSsoToken()
		{
			return lwSsoCookie;
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

		public void Disconnect()
		{
			try
			{
				ResponseWrapper wrapper = ExecutePost(DISCONNECT_URL, null, null);

			}
			catch (Exception)
			{
				//do nothing
			}

			// Reset cookies container to erase any existing cookies of the previous session.
			lwSsoCookie = null;
			octaneUserCookie = null;
		}

		public bool IsConnected()
		{
			return GetLwSsoToken() != null;
		}

		private HttpWebRequest CreateRequest(string restRelativeUri, RequestType requestType, RequestConfiguration additionalRequestConfiguration)
		{
			String url = host + restRelativeUri;
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

			//add cookies
			String cookieDomain = request.Address.Host;
			String cookiePath = "/";

			request.CookieContainer = new CookieContainer();
			request.CookieContainer.Add(new Cookie(LWSSO_COOKIE_NAME, lwSsoCookie, cookiePath, cookieDomain));
			request.CookieContainer.Add(new Cookie(OCTANE_USER_COOKIE_NAME, octaneUserCookie, cookiePath, cookieDomain));

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
				var response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(AwaitContinueOnCapturedContext);
				using (var streamReader = new StreamReader(response.GetResponseStream()))
				{
					responseWrapper.Data = streamReader.ReadToEnd();
				}

				responseWrapper.StatusCode = response.StatusCode;
				SaveCookies(response);

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

					String body = string.Empty;
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
						RestExceptionInfo exceptionInfo = jsSerializer.Deserialize<RestExceptionInfo>(body);
						throw new MqmRestException(exceptionInfo, response.StatusCode, ex);
					}
					catch
					{
						throw new GeneralHttpException(body, response.StatusCode);
					}
				}
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

			if ((requestType == RequestType.Post || requestType == RequestType.Update) && !String.IsNullOrEmpty(data))
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

			try
			{
				return await DoSendAsync(request).ConfigureAwait(AwaitContinueOnCapturedContext);
			}
			//catch - intended to handle the case LWSSO is expired, we will try reconnect and resend original request
			catch (InvalidCredentialException e)
			{
				try
				{
					bool reconnected = allowReconnect && Reconnect();
					if (!reconnected)
					{
						throw e;
					}
					// we reconnected, 
					// but await can't exist in a catch because the CLR would lose the ambient exception.
					// We don’t need the ambient exception (i.e. we don't "throw;", 
					// so we need to trick it out by putting retry logic after the catch
				}
				catch
				{
					//if reconnect throwed any exception - we rethrow original exception
					throw e;
				}
			}
			//resend after reconnect
			return await SendAsync(restRelativeUri, queryParams, requestType, data, false, additionalRequestConfiguration)
				   .ConfigureAwait(AwaitContinueOnCapturedContext);
		}

		public ResponseWrapper SendMultiPart(string restRelativeUrl, Byte[] binaryContent, string binaryContentType, string fileName, string entityData)
		{
			return SendMultiPartAsync(restRelativeUrl, binaryContent, binaryContentType, fileName, entityData).Result;
		}

		public async Task<ResponseWrapper> SendMultiPartAsync(string restRelativeUrl, Byte[] binaryContent, string binaryContentType, string fileName, string entityData)
		{
			string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
			byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

			HttpWebRequest wr = CreateRequest(restRelativeUrl, RequestType.MultiPart, null);
			wr.ContentType += boundary;

			Stream rs = await wr.GetRequestStreamAsync().ConfigureAwait(AwaitContinueOnCapturedContext);

			string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"blob\"\r\nContent-Type: application/json\r\n\r\n{1}";

			rs.Write(boundarybytes, 0, boundarybytes.Length);
			string formItem = string.Format(formdataTemplate, "entity", entityData);
			byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formItem);
			rs.Write(formitembytes, 0, formitembytes.Length);

			rs.Write(boundarybytes, 0, boundarybytes.Length);

			string headerTemplate = "Content-Disposition: form-data; name=\"content\"; filename=\"{0}\"\r\nContent-Type: {1}\r\n\r\n";
			string header = string.Format(headerTemplate, fileName, binaryContentType);
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

			return await DoSendAsync(wr).ConfigureAwait(AwaitContinueOnCapturedContext);
		}

	}
}
