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

namespace MicroFocus.Adm.Octane.Api.Core.Connector.Authentication
{
    public interface ConnectionListener
    {
        /// <summary>
        /// Open the system browser redirecting to the given url
        /// </summary>
        /// <param name="url"></param>
        void OpenBrowser(String url);

        /// <summary>
        /// Closes the browser after connection succeeded or time to connect elapsed
        /// </summary>
        /// <param name="url"></param>
        void CloseBrowser();

        /// <summary>
        /// Returns whether the browser dialog is open or closed
        /// </summary>
        /// <returns>boolean representing if the dialog is closed</returns>
        bool IsOpen();
        
        /// <summary>
        /// Updates the timeout on the connection listener
        /// </summary>
        /// <param name="timeout"></param>
        void UpdateTimeout(int timeout);
    }
}
