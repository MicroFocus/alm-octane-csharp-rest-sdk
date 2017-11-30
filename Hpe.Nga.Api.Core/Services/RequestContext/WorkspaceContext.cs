﻿// (c) Copyright 2016 Hewlett Packard Enterprise Development LP

// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.

// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,

// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hpe.Nga.Api.Core.Services.RequestContext
{
	/// <summary>
	/// Define URL for specific workspace context
	/// </summary>
	public class WorkspaceContext : IRequestContext
	{
		public long WorkspaceId { get; set; }

		public long SharedSpaceId { get; set; }

		public WorkspaceContext(long sharedSpaceId, long workspaceId)
		{
			WorkspaceId = workspaceId;
			SharedSpaceId = sharedSpaceId;
		}

		public WorkspaceContext(string sharedSpaceId, string workspaceId)
		{
			WorkspaceId = long.Parse(workspaceId);
			SharedSpaceId = long.Parse(sharedSpaceId);
		}

		public static WorkspaceContext Create(long sharedSpaceId, long workspaceId)
		{
			return new WorkspaceContext(sharedSpaceId, workspaceId);
		}

		public static WorkspaceContext Create(string sharedSpaceId, string workspaceId)
		{
			return new WorkspaceContext(sharedSpaceId, workspaceId);
		}


		public virtual string GetPath()
		{
			return $"/api/shared_spaces/{SharedSpaceId}/workspaces/{WorkspaceId}";
		}

		public override string ToString()
		{
			return $"Shared Space Id = {SharedSpaceId}; Workspace Id : {WorkspaceId}";
		}
	}
}
