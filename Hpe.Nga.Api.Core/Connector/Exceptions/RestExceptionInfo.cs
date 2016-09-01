using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hpe.Nga.Api.Core.Services.Core;

namespace Hpe.Nga.Api.Core.Connector.Exceptions
{
    public class RestExceptionInfo
    {
        public string error_code { get; set; }
        public string correlation_id { get; set; }
        public string description { get; set; }
        public string description_translated { get; set; }
        public Dictionary<String,String> properties { get; set; }
    }
}
