using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.Api.Core.Connector
{
	public class OngoingRequest
	{
		public HttpWebRequest Request { get; private set; }
		public DateTime Started { get; private set; }

		public OngoingRequest(HttpWebRequest request, DateTime started)
		{
			this.Request = request;
			this.Started = started;
		}

		public static OngoingRequest Create(HttpWebRequest request)
		{
			return new OngoingRequest(request, DateTime.Now);


		}
	}
}
