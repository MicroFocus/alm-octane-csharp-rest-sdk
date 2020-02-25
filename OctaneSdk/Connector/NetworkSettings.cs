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

using System.Net;

namespace MicroFocus.Adm.Octane.Api.Core.Connector
{
    public static class NetworkSettings
    {
        public static void EnableAllSecurityProtocols()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// Use it if you need to work with HTTPS while server has improper sertificates
        /// </summary>
        public static void IgnoreServerCertificateValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        }

        /// <summary>
        /// 
        /// Custom proxy settings
        /// </summary>
        public static IWebProxy CustomProxy { get; set; }

    }
}
