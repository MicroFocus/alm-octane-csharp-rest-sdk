using System.Collections.Generic;


namespace Hpe.Nga.Api.Core.Connector
{
    public class RequestConfiguration
    {
        public bool GZipCompression { get; set; }

        public int? Timeout { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public static RequestConfiguration Create()
        {
            return new RequestConfiguration();
        }

        public RequestConfiguration SetGZipCompression(bool compress)
        {
            this.GZipCompression = compress;
            return this;
        }

        public RequestConfiguration SetTimeout(int? timeout)
        {
            this.Timeout = timeout;
            return this;
        }

        public RequestConfiguration AddHeader(string name, string value)
        {
            if (Headers == null)
            {
                Headers = new Dictionary<string, string>();
            }
            this.Headers.Add(name, value);

            return this;
        }

    }
}
