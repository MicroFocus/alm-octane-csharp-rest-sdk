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



namespace MicroFocus.Adm.Octane.Api.Core.Services.RequestContext
{
	/// <summary>
	/// Define URL for specific sharedspace context
	/// </summary>
	public class SharedSpaceContext : IRequestContext
	{
		public long SharedSpaceId { get; set; }

		public SharedSpaceContext(long sharedSpaceId)
		{
			SharedSpaceId = sharedSpaceId;
		}

		public SharedSpaceContext(string sharedSpaceId)
		{
			SharedSpaceId = long.Parse(sharedSpaceId);
		}

		public static SharedSpaceContext Create(long sharedSpaceId)
		{
			return new SharedSpaceContext(sharedSpaceId);
		}

		public static SharedSpaceContext Create(string sharedSpaceId)
		{
			return new SharedSpaceContext(sharedSpaceId);
		}

		public virtual string GetPath()
		{
			return string.Format("/api/shared_spaces/{0}", SharedSpaceId);
		}

		public override string ToString()
		{
			return "Shared Space Id = " + SharedSpaceId;
		}
	}
}
