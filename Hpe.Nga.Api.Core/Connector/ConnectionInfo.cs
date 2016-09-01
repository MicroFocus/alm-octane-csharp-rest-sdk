using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hpe.Nga.Api.Core.Connector
{
    /// <summary>
    /// POCO class for connection data that is sent to NGA server during calling to <see cref="Connect"/> method.
    /// </summary>
    public class ConnectionInfo
    {
        public string user { get; set; }
        public string password { get; set; }

       /* public string enable_csrf
        {
            get
            {
                return "true";
            }
        }*/


        public ConnectionInfo()
        {
        }

        public ConnectionInfo(String user, string password)
        {
            this.user = user;
            this.password = password;
        }
    }
}
