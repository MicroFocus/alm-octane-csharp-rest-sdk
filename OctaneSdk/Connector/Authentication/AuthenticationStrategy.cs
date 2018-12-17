using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
    }
}
