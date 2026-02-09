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


using System;
using System.Net;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Connector.Authentication
{
    public interface AuthenticationStrategy
    {
        Task<bool> ConnectAsync(string host);

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
        Task<string> GetWorkspaceUser();

        /// <summary>
        ///  Test the connection to the octane server
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        Task<bool> TestConnection(string host);

    }
}
