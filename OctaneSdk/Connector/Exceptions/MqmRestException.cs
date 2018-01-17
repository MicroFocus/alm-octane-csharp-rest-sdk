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
using System.Collections.Generic;
using System.Net;

namespace MicroFocus.Adm.Octane.Api.Core.Connector.Exceptions
{
	public class MqmRestException : GeneralHttpException
	{
		RestExceptionInfo exceptionInfo;

		public MqmRestException(RestExceptionInfo exceptionInfo, HttpStatusCode statusCode, Exception innerException) :
			base(exceptionInfo.description, statusCode, innerException)
		{
			this.exceptionInfo = exceptionInfo;
		}

		public String ErrorCode
		{
			get
			{
				return exceptionInfo.error_code;
			}
		}

		public String CorrelationInfo
		{
			get
			{
				return exceptionInfo.correlation_id;
			}
		}

		public String Description
		{
			get
			{
				return exceptionInfo.description;
			}
		}

		public String DescriptionTranslated
		{
			get
			{
				return exceptionInfo.description_translated;
			}
		}

		public Dictionary<String, String> Properties
		{
			get
			{
				return exceptionInfo.properties;
			}
		}
	}
}
