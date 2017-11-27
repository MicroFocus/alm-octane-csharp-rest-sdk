using System.Collections.Generic;


namespace Hpe.Nga.Api.Core.Connector
{
    public class RequestAdditionalData
    {
        public bool IsGZip { get; set; }

        public int? Timeout { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public static RequestAdditionalData Create()
        {
            return new RequestAdditionalData();
        }

        public RequestAdditionalData SetIsGZip(bool b)
        {
            this.IsGZip = b;
            return this;
        }

        public RequestAdditionalData SetTimeout(int? timeout)
        {
            this.Timeout = timeout;
            return this;
        }

        public RequestAdditionalData AddHeader(string name, string value)
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
