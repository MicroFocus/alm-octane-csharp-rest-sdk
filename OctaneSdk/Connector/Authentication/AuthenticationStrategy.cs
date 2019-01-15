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


using System;
using System.Net;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Connector.Authentication
{
    public interface AuthenticationStrategy
    {
        Task<bool> ConnectAsync(String host);

        bool IsConnected();

       /// <summary>
       /// Disconnect. No exceptions are expected
       /// </summary>
       /// <returns></returns>
        Task<bool> DisconnectAsync();

        /// <summary>
        /// Set all required authentication cookies/headers on request
        /// 
        /// </summary>
        /// <param name="request"></param>
        void PrepareRequest(HttpWebRequest request);

        /// <summary>
        /// Retrieve authentication information from response
        /// </summary>
        /// <param name="response"></param>
        void OnResponse(HttpWebResponse response);

        /// <summary>
        /// Retrieve the workspace user
        /// </summary>
        Task<String> GetWorkspaceUser();

        /// <summary>
        ///  Test the connection to the octane server
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        Task<bool> TestConnection(string host);

    }
}
