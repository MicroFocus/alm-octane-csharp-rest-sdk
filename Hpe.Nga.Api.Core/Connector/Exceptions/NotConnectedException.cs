using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hpe.Nga.Api.Core.Connector.Exceptions
{
    /// <summary>
    /// The exeption is thrown on trial to send request without successful calling  <see cref="RestConnector.Connect"/>
    /// </summary>
    public class NotConnectedException : Exception
    {
    }
}
