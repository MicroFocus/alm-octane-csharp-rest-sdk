using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hpe.Nga.Api.Core.Connector
{
    public class ResponseWrapper
    {
        public String Data { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}
