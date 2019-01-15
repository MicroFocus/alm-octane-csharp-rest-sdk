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

namespace MicroFocus.Adm.Octane.Api.Core.Connector
{
	/// <summary>
	/// POCO class for connection data that is sent to NGA server during calling to <see cref="Connect"/> method.
	/// Uses the client_id/client_secret method
	/// </summary>
	public class APIKeyConnectionInfo : ConnectionInfo
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }

        public APIKeyConnectionInfo(String client_id, string client_secret)
        {
            this.client_id = client_id;
            this.client_secret = client_secret;
        }
    }
}
